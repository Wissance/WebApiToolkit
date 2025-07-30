using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    /// <summary>
    ///    This is a file manager that works with files 
    /// </summary>
    public class AWSCompatibleFileStorageManager : IAWSCompatibleFileStorageManager, IDisposable
    {
        public AWSCompatibleFileStorageManager(IDictionary<string, S3StorageSettings> sources, ILoggerFactory loggerFactory)
        {
            _sources = sources;
            _logger = loggerFactory.CreateLogger<AWSCompatibleFileStorageManager>();
            foreach (KeyValuePair<string, S3StorageSettings> source in _sources)
            {
                try
                {
                    S3StorageSettings settings = source.Value;
                    IAmazonS3 s3Client = new AmazonS3Client(
                        new BasicAWSCredentials(settings.AccessKey, settings.SecretAccessKey),
                        new AmazonS3Config
                        {
                            ServiceURL = settings.Endpoint,
                        });
                    _s3Clients[source.Key] = s3Client;
                }
                catch (AmazonClientException e)
                {
                    _logger.LogError(
                        $"Incorrect S3 Storage configuration: {e.Message} , ensure S3Storage section has correct values");
                }
            }
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string,IAmazonS3> client in _s3Clients)
            {
                client.Value.Dispose();
            }
        }

        public OperationResultDto<IList<string>> GetSources()
        {
            return new OperationResultDto<IList<string>>(true, (int) HttpStatusCode.OK, String.Empty,
                _sources.Keys.ToList());
        }
        
        public async Task<OperationResultDto<IList<string>>> GetBucketsAsync(string source)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResultDto<bool>> CreateBucketAsync(string source, string bucketName)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResultDto<bool>> DeleteBucketAsync(string source, string bucketName)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync(string source, string path = ".",
            IDictionary<string, string>additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath,
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName,
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath, 
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent,
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath, IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> UpdateFileAsync(string source, string filePath, MemoryStream fileContent, 
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        public event DirectorySuccessfullyCreatedHandler OnDirectoryCreated;
        public event DirectorySuccessfullyDeletedHandler OnDirectoryDeleted;
        public event FileSuccessfullyCreatedHandler OnFileCreated;
        public event FileSuccessfullyDeletedHandler OnFileDeleted;

        private readonly IDictionary<string, S3StorageSettings> _sources = new Dictionary<string, S3StorageSettings>();
        private readonly ConcurrentDictionary<string, IAmazonS3> _s3Clients = new ConcurrentDictionary<string, IAmazonS3>();
        private readonly ILogger<AWSCompatibleFileStorageManager> _logger;
    }
}