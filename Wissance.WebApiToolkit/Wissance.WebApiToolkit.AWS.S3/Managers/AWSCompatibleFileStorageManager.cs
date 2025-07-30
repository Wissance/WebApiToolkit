using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.AWS.S3.Settings;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.AWS.S3.Managers
{
    /// <summary>
    ///    This is a S3 Cloud file manager that are compatible with AWS
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

        /// <summary>
        ///     Returns list of the sources (keys)
        /// </summary>
        /// <returns>return list of sources (dictionary keys)</returns>
        public OperationResultDto<IList<string>> GetSources()
        {
            return new OperationResultDto<IList<string>>(true, (int) HttpStatusCode.OK, String.Empty,
                _sources.Keys.ToList());
        }
        
        /// <summary>
        ///     Returns list of buckets for the specified source
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <returns>returns list of buckets names</returns>
        public async Task<OperationResultDto<IList<string>>> GetBucketsAsync(string source)
        {
            try
            {
                IAmazonS3 s3Client = _s3Clients[source];
                ListBucketsResponse bucketsResp = await s3Client.ListBucketsAsync();
                IList<string> bucketsNames = bucketsResp.Buckets.Select(b => b.BucketName).ToList();
                return new OperationResultDto<IList<string>>(true, (int) HttpStatusCode.OK, String.Empty, bucketsNames);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during getting buckets list: {e.Message}");
                _logger.LogDebug(e.ToString());
                return null;
            }
        }

        /// <summary>
        ///     Create bucket in the source. 
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="bucketName">name of bucket to create</param>
        /// <returns>returns bool if bucket was created or if it already exists</returns>
        public async Task<OperationResultDto<bool>> CreateBucketAsync(string source, string bucketName)
        {
            try
            {
                IAmazonS3 s3Client = _s3Clients[source];
                bool isExists = await CheckBucketExistsAsync(source, bucketName);
                if (isExists)
                    return new OperationResultDto<bool>(true, (int) HttpStatusCode.OK, String.Empty, true);
                PutBucketResponse response = await s3Client.PutBucketAsync(bucketName);
                bool created = response.HttpStatusCode == HttpStatusCode.OK;
                int statusCode = created ? (int) HttpStatusCode.Created : (int) response.HttpStatusCode;
                return new OperationResultDto<bool>(true, statusCode, String.Empty, created);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during bucket with name \"{bucketName}\" creation, error: \"{e.Message}\"");
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, String.Empty, false);
            }
        }

        /// <summary>
        ///     Deletes bucket in the source.
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="bucketName">name of bucket to delete</param>
        /// <returns></returns>
        public async Task<OperationResultDto<bool>> DeleteBucketAsync(string source, string bucketName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync(string source, string path = ".",
            IDictionary<string, string>additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filePath"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath,
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <param name="dirName"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName,
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dirPath"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath, 
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent,
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filePath"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath, IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<OperationResultDto<bool>> UpdateFileAsync(string source, string filePath, MemoryStream fileContent, 
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }
        
        private async Task<bool> CheckBucketExistsAsync(string source, string bucketName)
        {
            try
            {
                OperationResultDto<IList<string>> buckets = await GetBucketsAsync(source);
                return buckets.Data.Contains(bucketName);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public event DirectorySuccessfullyCreatedHandler OnDirectoryCreated;
        public event DirectorySuccessfullyDeletedHandler OnDirectoryDeleted;
        public event FileSuccessfullyCreatedHandler OnFileCreated;
        public event FileSuccessfullyDeletedHandler OnFileDeleted;

        private readonly IDictionary<string, S3StorageSettings> _sources = new Dictionary<string, S3StorageSettings>();
        private readonly IDictionary<string, IAmazonS3> _s3Clients = new ConcurrentDictionary<string, IAmazonS3>();
        private readonly ILogger<AWSCompatibleFileStorageManager> _logger;
    }
}