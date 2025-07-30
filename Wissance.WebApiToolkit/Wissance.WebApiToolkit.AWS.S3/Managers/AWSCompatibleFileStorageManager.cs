using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.AWS.S3.Settings;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.AWS.S3.Managers
{
    public class AWSCompatibleFileStorageManager : IFileManager
    {
        public AWSCompatibleFileStorageManager(S3StorageSettings settings, ILoggerFactory loggerFactory)
        {
            _settings = settings;
            _logger = loggerFactory.CreateLogger<AWSCompatibleFileStorageManager>();
            try
            {
                _s3Client = new AmazonS3Client(new BasicAWSCredentials(_settings.AccessKey, _settings.SecretAccessKey),
                    new AmazonS3Config
                    {
                        ServiceURL = _settings.Endpoint,
                    });
            }
            catch(AmazonClientException e)
            {
                _logger.LogError($"Incorrect S3 Storage configuration: {e.Message} , ensure S3Storage section has correct values");
            }
        }

        public OperationResultDto<IList<string>> GetSources()
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync(string source, string path = ".")
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> UpdateFileAsync(string source, string filePath, MemoryStream fileContent)
        {
            throw new System.NotImplementedException();
        }

        public event DirectorySuccessfullyCreatedHandler OnDirectoryCreated;
        public event DirectorySuccessfullyDeletedHandler OnDirectoryDeleted;
        public event FileSuccessfullyCreatedHandler OnFileCreated;
        public event FileSuccessfullyDeletedHandler OnFileDeleted;
        
        private readonly IAmazonS3 _s3Client;
        private readonly S3StorageSettings _settings;
        private readonly ILogger<AWSCompatibleFileStorageManager> _logger;
    }
}