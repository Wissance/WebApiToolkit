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
        [InlineData(WissanceYandexTestBucket, "src", "cli/")]
        public async Task CreateDirectorySuccessfully(string bucket, string path, string dirName)
        {
            OperationResultDto<string> result = await _manager.CreateDirAsync(WissanceYandexTestSource, path, dirName, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(result.Success);
            // todo(UMV) : check result path
            string dirPath = $"{path}/{dirName}";
            OperationResultDto<bool> rmResult = await _manager.DeleteDirAsync(WissanceYandexTestSource, dirPath, new Dictionary<string, string>()
            {
                {AWSCompatibleCloudFileStorageManager.BucketParam, bucket}
            });
            Assert.True(rmResult.Success);
        }

        private const string WissanceYandexTestSource = "wissance";
        private const string WissanceYandexTestBucket = "y-s3-test-bucket";
        
        private readonly IAWSCompatibleFileStorageManager _manager;
    }
}