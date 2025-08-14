using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Minio.S3.Managers;
using Wissance.WebApiToolkit.Minio.S3.Settings;

namespace Wissance.WebApiToolkit.Minio.S3.Tests.Managers
{
    public class TestMinioFileStorageManager : IDisposable
    {
        public TestMinioFileStorageManager()
        {
            _manager = new MinioFileStorageManager(new Dictionary<string, MinioSettings>()
            {
                {
                    LocalMinioSource, new MinioSettings()
                    {
                        Endpoint = "127.0.0.1:9000",
                        AccessKey = "minioadmin",
                        SecretAccessKey = "minioadmin"
                    }
                }
            }, new LoggerFactory());
        }

        public void Dispose()
        {
            _manager.Dispose();
        }

        // todo(UMV) : MinIO stopped to have free license, therefore this client is on pause
        [Fact]
        public async Task FullCycleTest()
        {
            string testBucket = $"WebApiToolkit_Test_Bucket_{Guid.NewGuid()}".ToLower().Replace("_", "-");
            OperationResultDto<bool> bucketCreateResult = await _manager.CreateBucketAsync(LocalMinioSource, testBucket);
            Assert.True(bucketCreateResult.Success);
            OperationResultDto<bool> bucketDeleteResult = await _manager.DeleteBucketAsync(LocalMinioSource, testBucket);
            Assert.True(bucketDeleteResult.Success);
        }

        private const string LocalMinioSource = "local_minio";

        private readonly IMinioFileStorageManager _manager;
    }
}