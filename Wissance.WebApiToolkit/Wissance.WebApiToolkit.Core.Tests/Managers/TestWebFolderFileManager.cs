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
        
        public static string GenerateAlphaDigitalStr(int length)
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