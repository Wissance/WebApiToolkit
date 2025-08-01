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

        private const string WissanceYandexTestSource = "wissance";
        private const string WissanceYandexTestBucket = "y-s3-test-bucket";
        
        private readonly IAWSCompatibleFileStorageManager _manager;
    }
}