using System.Buffers.Text;
using System.Text;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wissance.WebApiToolkit.AWS.S3.Managers;
using Wissance.WebApiToolkit.AWS.S3.Settings;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.AWS.S3.Tests.Managers
{
    public class TestAwsCompatibleCloudFileStorageManagerOnYandex : IDisposable
    {
        /// <summary>
        ///     These tests were written for testing IAWSCompatibleFileStorageManager operation
        ///     with Yandex Object Storage
        ///     1. It is required to have service account with role storage.editor
        ///        Don't forget to save key and secret to file settings.json in this project root dir and set them to copy to output
        ///        path, this file has a following content (values changed):
        ///        {
        ///            "AccessKey": "YassEZBPassMGzTelzassigfQ",
        ///            "SecretAccessKey": "YCMeYuWMassSWPBAasselV7assQ0assDNUassdkS"
        ///        }
        ///     2. Prior to tests you have to create following buckets:
        ///         - y-s3-test-bucket
        ///         - y-s3-test-bucket-2
        ///     These tests depends on S3 predefined structure, bucket "y-s3-test-bucket-2" must have the following
        ///     structure:
        ///     /
        ///     --/artifacts
        ///       --/archives
        ///       --/txt
        ///         --/test_data.txt  (content: test_data:1234567890 qwertyuiopasdfghjklzxcvbnm)
        ///         --/test_data2.txt (content: 01234567890987654321)
        ///     --/bin
        ///     --/docs
        ///     --/src
        /// </summary>
        public TestAwsCompatibleCloudFileStorageManagerOnYandex()
        {
            // todo(UMV): take keys from env vars 
            string jsonStr = File.ReadAllText("./settings.json");
            // reading trimmed key && secret from settings.json (non-tracking)
            S3StorageSettings keyAndSecrets = JsonConvert.DeserializeObject<S3StorageSettings>(jsonStr);
            S3StorageSettings yandexTestSettings = new S3StorageSettings()
            {
                StorageType = S3StorageType.Yandex,
                AccessKey = keyAndSecrets.AccessKey,
                SecretAccessKey = keyAndSecrets.SecretAccessKey,
                Endpoint = "https://storage.yandexcloud.net"
            };
            _manager = new AWSCompatibleCloudFileStorageManager(new Dictionary<string, S3StorageSettings>()
            {
                {WissanceYandexTestSource, yandexTestSettings}
            }, new LoggerFactory());
        }

        public void Dispose()
        {
            _manager.Dispose();
        }

        [Fact]
        public async Task TestGetBuckets()
        {
            OperationResultDto<IList<string>> result = await _manager.GetBucketsAsync(WissanceYandexTestSource);
            Assert.True(result.Success);
            Assert.Contains(result.Data, b => b == WissanceYandexTestBucket);
        }

        [Theory]
        [InlineData("y-new-bucket-000")]
        public async Task TestCreateAndDeleteBucket(string newBucket)
        {
            OperationResultDto<bool> createResult = await _manager.CreateBucketAsync(WissanceYandexTestSource, newBucket);
            Assert.True(createResult.Success);
            MemoryStream fileContent = new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0});
            string path = "";
            string fileName = "go.mod";
            OperationResultDto<string> result = await _manager.CreateFileAsync(WissanceYandexTestSource, path, fileName, fileContent,
                new Dictionary<string, string>()
                {
                    {AWSCompatibleCloudFileStorageManager.BucketParam, newBucket}
                });
            Assert.True(result.Success);
            Assert.Equal(fileName, result.Data);
            // non empty bucket can't be deleted
            OperationResultDto<bool> rmResult = await _manager.DeleteFileAsync(WissanceYandexTestSource, fileName, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, newBucket}
            });
            Assert.True(rmResult.Success);
            OperationResultDto<bool> deleteResult = await _manager.DeleteBucketAsync(WissanceYandexTestSource, newBucket);
            Assert.True(deleteResult.Success);
        }

        [Theory]
        [InlineData(WissanceYandexTestBucket, "", 4)]
        [InlineData(WissanceYandexTestBucket, "artifacts", 2)]
        [InlineData(WissanceYandexTestBucket, "artifacts/txt", 2)]
        [InlineData(WissanceYandexTestBucket, "artifacts/archives", 0)]
        public async Task TestGetFiles(string bucket, string path, int expectedItems)
        {
            OperationResultDto<IList<TinyFileInfo>> result = await _manager.GetFilesAsync(WissanceYandexTestSource, path, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(result.Success);
            Assert.Equal(expectedItems, result.Data.Count);
        }

        [Theory]
        [InlineData(WissanceYandexTestBucket, "artifacts/txt/test_data2.txt", "01234567890987654321")]
        public async Task TestGetFileContent(string bucket, string path, string expectedContent)
        {
            OperationResultDto<MemoryStream> result = await _manager.GetFileContentAsync(WissanceYandexTestSource, path, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(result.Success);
            byte[] buffer = result.Data.ToArray();
            string actualContent = UTF8Encoding.UTF8.GetString(buffer);
            Assert.Equal(expectedContent, actualContent);
        }

        [Theory]
        [InlineData(WissanceYandexTestBucket, "src", "cli")]
        public async Task CreateDirectorySuccessfully(string bucket, string path, string dirName)
        {
            OperationResultDto<string> result = await _manager.CreateDirAsync(WissanceYandexTestSource, path, dirName, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(result.Success);
            string dirPath = $"{path}/{dirName}";
            Assert.Equal($"{dirPath}/", result.Data);
            // get list
            OperationResultDto<IList<TinyFileInfo>> files = await _manager.GetFilesAsync(WissanceYandexTestSource, path,
                new Dictionary<string, string>()
                {
                    {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
                });
            Assert.True(files.Success);
            Assert.True(files.Data.Any(f => f.Name == dirName));
            OperationResultDto<bool> rmResult = await _manager.DeleteDirAsync(WissanceYandexTestSource, dirPath, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(rmResult.Success);
            files = await _manager.GetFilesAsync(WissanceYandexTestSource, path,
                new Dictionary<string, string>()
                {
                    {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
                });
            Assert.True(files.Success);
            Assert.False(files.Data.Any(f => f.Name == dirName));
        }

        [Theory]
        [InlineData(WissanceYandexTestBucket, "src", "go.mod")]
        public async Task CreateFileSuccessfully(string bucket, string path, string fileName)
        {
            MemoryStream fileContent = new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0});
            OperationResultDto<string> result = await _manager.CreateFileAsync(WissanceYandexTestSource, path, fileName, fileContent,
                new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(result.Success);
            // todo(UMV) : check result path
            string filePath = $"{path}/{fileName}";
            Assert.Equal(filePath, result.Data);
            OperationResultDto<IList<TinyFileInfo>> files = await _manager.GetFilesAsync(WissanceYandexTestSource, path,
                new Dictionary<string, string>()
                {
                    {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
                });
            Assert.True(files.Success);
            Assert.True(files.Data.Any(f => f.Name == fileName));
            OperationResultDto<bool> rmResult = await _manager.DeleteFileAsync(WissanceYandexTestSource, filePath, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(rmResult.Success);
        }

        [Theory]
        [InlineData(WissanceYandexTestBucket, "src", "go.mod")]
        public async Task TestUpdateFileSuccessfully(string bucket, string path, string fileName)
        {
            byte[] expectedContent = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 0};
            MemoryStream fileContent = new MemoryStream(expectedContent);
            OperationResultDto<string> result = await _manager.CreateFileAsync(WissanceYandexTestSource, path, fileName, fileContent,
                new Dictionary<string, string>()
                {
                    {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
                });
            Assert.True(result.Success);
            // todo(UMV) : check result path
            string filePath = $"{path}/{fileName}";
            Assert.Equal(filePath, result.Data);
            OperationResultDto<MemoryStream> contentResult = await _manager.GetFileContentAsync(WissanceYandexTestSource, filePath, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(result.Success);
            byte[] buffer = contentResult.Data.ToArray();
            string actualContent = UTF8Encoding.UTF8.GetString(buffer);
            Assert.Equal(UTF8Encoding.UTF8.GetString(expectedContent), actualContent);

            expectedContent = new byte[] {2, 4, 8, 16, 32, 64, 128, 255};
            fileContent = new MemoryStream(expectedContent);
            OperationResultDto<bool> updateResult = await _manager.UpdateFileAsync(WissanceYandexTestSource, filePath, fileContent, 
                new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(updateResult.Success);
            contentResult = await _manager.GetFileContentAsync(WissanceYandexTestSource, filePath, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(result.Success);
            buffer = contentResult.Data.ToArray();
            actualContent = UTF8Encoding.UTF8.GetString(buffer);
            Assert.Equal(UTF8Encoding.UTF8.GetString(expectedContent), actualContent);

            OperationResultDto<bool> rmResult = await _manager.DeleteFileAsync(WissanceYandexTestSource, filePath, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(rmResult.Success);
        }

        private const string WissanceYandexTestSource = "wissance";
        private const string WissanceYandexTestBucket = "y-s3-test-bucket-2";
        
        private readonly IAWSCompatibleFileStorageManager _manager;
    }
}