namespace Wissance.WebApiToolkit.AWS.S3.Settings
{
    public class S3StorageSettings
    {
        public S3StorageType StorageType { get; set; }
        public string AccountId { get; set; }
        public string Endpoint { get; set; }
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretAccessKey { get; set; }
        
        public string Bucket { get; set; }

        // public const string AwsStorageType = "AWS";
        // public const string CloudflareStorageType = "Cloudflare";
    }
}