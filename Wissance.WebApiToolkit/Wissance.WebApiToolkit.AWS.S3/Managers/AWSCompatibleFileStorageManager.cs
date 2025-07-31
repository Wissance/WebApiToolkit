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
    ///    This is a S3 Cloud file manager that are compatible with AWS. This interface was not designed
    ///    to be working with a heavy files (hundreads of megabytes or even gigabytes)
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
                string msg = $"An error occurred during bucket with name \"{bucketName}\" creation, error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
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
            try
            {
                IAmazonS3 s3Client = _s3Clients[source];
                bool isExists = await CheckBucketExistsAsync(source, bucketName);
                if (!isExists)
                    return new OperationResultDto<bool>(true, (int) HttpStatusCode.OK, String.Empty, true);
                DeleteBucketResponse response = await s3Client.DeleteBucketAsync(bucketName);
                bool result = response.HttpStatusCode == HttpStatusCode.NoContent;
                return new OperationResultDto<bool>(true, (int) HttpStatusCode.NoContent, String.Empty, result);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during bucket with name \"{bucketName}\" remove, error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
            }
        }

        /// <summary>
        ///    This method return files and folder from bucket, Bucket MUST BE passed though additionalParams
        ///    with BucketParam key. This method return latest versions of files but to get all versions
        ///    of files we should use another method. This case is not implemented, but it could be implemented
        ///    by specifying additionalParams param. We expect to work with small files (< 50-100 Mb).
        ///    For "heavy" files should be created a separate implementation of this interface
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="path">Path to directory (should not start from . or ./), however we are handling such case</param>
        /// <param name="additionalParams">Additional params contains Bucket as a param</param>
        /// <returns></returns>
        public async Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync(string source, string path = ".",
            IDictionary<string, string>additionalParams = null)
        {
            string bucket = "";
            IList <TinyFileInfo> objs = new List<TinyFileInfo>();
            try
            {
                bucket = additionalParams != null && additionalParams.ContainsKey(BucketParam)
                    ? additionalParams[BucketParam]
                    : "";
                // Root means . or / or ./
                bool isRoot = path.Length <= 2 && (path[0] == '.' || path[0] == '/');
                string realPath = !isRoot ? path.Trim(new []{'.'}).Trim(new []{'/'}) : path;

                IAmazonS3 s3Client = _s3Clients[source];
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = bucket,
                };
                ListObjectsV2Response response;
                do
                {
                    response = await s3Client.ListObjectsV2Async(request);

                    foreach (S3Object s3Object in response.S3Objects)
                    {
                        // Console.WriteLine($"Object Key: {s3Object.Key}, Size: {s3Object.Size}");
                        // 1. Filter by path, Key is a file name with path to file
                        if (!isRoot)
                        {
                            if (s3Object.Key.StartsWith(realPath))
                            {
                                // here we should clarify one thing
                                string[] partsOfPath = realPath.Split("/");
                                string[] parts = s3Object.Key.Split("/");
                                if (parts.Length - partsOfPath.Length <= 1)
                                {
                                    objs.Add(new TinyFileInfo(parts.Last(),
                                        !s3Object.Size.HasValue || s3Object.Size == 0,
                                        s3Object.Size ?? 0));
                                }
                            }
                        }
                        else
                        {
                            if (!s3Object.Key.Contains("/"))
                            {
                                objs.Add(new TinyFileInfo(s3Object.Key, !s3Object.Size.HasValue || s3Object.Size == 0, 
                                    s3Object.Size ?? 0));
                            }
                        }
                    }

                    // If the response is truncated, set the ContinuationToken for the next request
                    request.ContinuationToken = response.NextContinuationToken;

                } while (response.IsTruncated.HasValue &&
                         response.IsTruncated.Value); // Continue looping until all objects are retrieved

                return new OperationResultDto<IList<TinyFileInfo>>(true, (int) HttpStatusCode.OK, String.Empty, objs);

            }
            catch (Exception e)
            {
                string msg = $"An error occurred during getting object list for bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<IList<TinyFileInfo>>(false, (int) HttpStatusCode.InternalServerError, msg, null);
            }
        }

        /// <summary>
        ///    Reads file data as a binary stream and return as a MemoryStream. filePath must be equal to one of the Keys
        ///    from bucket.  Bucket MUST BE passed though additionalParams with BucketParam key.
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="filePath">Path to file in bucket (should not start from / ./ )</param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public async Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath,
            IDictionary<string, string>additionalParams)
        {
            string bucket = "";
            try
            {
                IAmazonS3 s3Client = _s3Clients[source];
                bucket = additionalParams[BucketParam];
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = filePath,
                };
                GetObjectResponse response = await s3Client.GetObjectAsync(request);
                byte[] bytes = new byte[0];
                using (BinaryReader binaryReader = new BinaryReader(response.ResponseStream))
                {
                    bytes = binaryReader.ReadBytes((int)response.ResponseStream.Length);
                }

                MemoryStream ms = new MemoryStream(bytes);
                return new OperationResultDto<MemoryStream>(true, (int) HttpStatusCode.OK, String.Empty, ms);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during getting file content from bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="path"></param>
        /// <param name="dirName"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public async Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName,
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="dirPath"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public async Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath, 
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent,
            IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="filePath"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath, IDictionary<string, string>additionalParams)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
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

        public const string BucketParam = "bucket";

        private readonly IDictionary<string, S3StorageSettings> _sources = new Dictionary<string, S3StorageSettings>();
        private readonly IDictionary<string, IAmazonS3> _s3Clients = new ConcurrentDictionary<string, IAmazonS3>();
        private readonly ILogger<AWSCompatibleFileStorageManager> _logger;
    }
}