namespace Wissance.WebApiToolkit.Minio.S3.Settings
{
    public class MinioSettings
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretAccessKey { get; set; }
        public bool IsSecure { get; set; }
    }
}