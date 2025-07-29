using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Tests.Managers
{
    public class TestWebFolderFileManager : IDisposable
    {
        public TestWebFolderFileManager()
        {
            _manager = new WebFolderFileManager(_webFolderTestSources, "source1", new LoggerFactory());
            RemoveTestWebFolders();
            CreateTestWebFolders();
        }

        public void Dispose()
        {
            RemoveTestWebFolders();
        }

        [Theory]
        [InlineData("source1", ".", false, 2)]
        [InlineData("source2", ".", false, 2)]
        public async Task TestGetAllFilesSuccessfully(string source, string path, bool exactMatch, int expectedNumber)
        {
            OperationResultDto<IList<TinyFileInfo>> actual = await _manager.GetFilesAsync(source, path);
            Assert.True(actual.Success);
            if (exactMatch)
                Assert.Equal(expectedNumber, actual.Data.Count);
            else
                Assert.True(actual.Data.Count >= expectedNumber);
        }

        [Fact]
        public async Task TestGetAllFilesFailsNoSource()
        {
            string source = $"source_{Guid.NewGuid()}";
            OperationResultDto<IList<TinyFileInfo>> result = await _manager.GetFilesAsync(source, ".");
            Assert.False(result.Success);
        }

        [Fact]
        public async Task TestGetFileContentSuccessfully()
        {
            OperationResultDto<MemoryStream> result = await _manager.GetFileContentAsync("source1", "test_file_1.txt");
            Assert.True(result.Success);
            byte[] data = result.Data.GetBuffer();
            Assert.True(data.Length >= 10);
            result.Data.Close();
        }

        
        [Theory]
        [InlineData("source1", ".", "newDir")]
        public async Task TestCreateDirectoryTreeSuccessfully(string source, string path, string dirName)
        {
            OperationResultDto<string> result = await _manager.CreateDirAsync(source, path, dirName);
            Assert.True(result.Success);
            string expectedPath = Path.GetFullPath(Path.Combine(_webFolderTestSources[source], path, dirName));
            Assert.Equal(expectedPath, result.Data);
            
            string subDirectory = "subDir2";
            result = await _manager.CreateDirAsync(source, dirName, subDirectory);
            Assert.True(result.Success);
            expectedPath = Path.GetFullPath(Path.Combine(_webFolderTestSources[source], dirName, subDirectory));
            Assert.Equal(expectedPath, result.Data);
            
            OperationResultDto<IList<TinyFileInfo>> filesResult = await _manager.GetFilesAsync(source, ".");
            Assert.True(filesResult.Data.Count >= 3);
            Assert.Equal(1, filesResult.Data.Count(f => f.IsDirectory));
        }

        [Fact]
        public async Task TestDeleteDirectory()
        {
            string source = "source1";
            string dirName = "newDir";
            OperationResultDto<string> result = await _manager.CreateDirAsync(source, ".", dirName);
            Assert.True(result.Success);
            OperationResultDto<IList<TinyFileInfo>> filesResult = await _manager.GetFilesAsync(source, ".");
            Assert.Equal(1, filesResult.Data.Count(f => f.IsDirectory));
            OperationResultDto<bool> rmResult = await _manager.DeleteDirAsync(source, dirName);
            Assert.True(rmResult.Success);
            filesResult = await _manager.GetFilesAsync(source, ".");
            Assert.Equal(0, filesResult.Data.Count(f => f.IsDirectory));
        }

        [Fact]
        public async Task TestCreateFile()
        {
        }
        
        [Fact]
        public async Task TestUpdateFile()
        {
        }
        
        [Fact]
        public async Task TestDeleteFile()
        {
        }

        private void CreateTestWebFolders()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            foreach (KeyValuePair<string, string> webFolder in _webFolderTestSources)
            {
                if (!Directory.Exists(webFolder.Value))
                    Directory.CreateDirectory(webFolder.Value);
                int initialFilesInFolder = rnd.Next(2, 3);
                for (int i = 0; i < initialFilesInFolder; i++)
                {
                    string testFileName = $"test_file_{i+1}.txt";
                    string fullTestFilePath = Path.Combine(webFolder.Value, testFileName);
                    string rndContent = GenerateAlphaDigitalStr(rnd.Next(10, 20));
                    File.WriteAllText(fullTestFilePath, rndContent);
                }
            }
        }
        
        private string GenerateAlphaDigitalStr(int length)
        {
            Random rnd = new Random((int)DateTime.UtcNow.Ticks);
            string randomStr = new string(Enumerable.Repeat(AlphabeticalDigitsCharset, length).Select(s => s[rnd.Next(s.Length)]).ToArray());
            return randomStr;
        }

        private void RemoveTestWebFolders()
        {
            foreach (KeyValuePair<string,string> webFolder in _webFolderTestSources)
            {
                if (Directory.Exists(webFolder.Value))
                {
                    Directory.Delete(webFolder.Value, true);
                }
            }
        }
        
        private const string AlphabeticalDigitsCharset = "abcdefghijklmnopqrstuvwxyz0123456789";

        private readonly IFileManager _manager;

        private readonly IDictionary<string, string> _webFolderTestSources = new Dictionary<string, string>()
        {
            {"source1", "./web/files/src1"},
            {"source2", "./web/files/src2"}
        };
    }
}