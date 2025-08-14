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
    ///    to be working with a heavy files (hundreds of megabytes or even gigabytes)
    ///    Things that MUST BE noted:
    ///    1. IFileManager in all methods contains last parameter IDictionary<string, string> with additional parameters
    ///       this implementation expects to pass at least bucket (with key in const BucketParam)
    ///    2. All object in S3 storage identified by key which is actually path, however, this path does not
    ///       starting from . or ./ or /, therefore all path here unlike of WebFolderFileManager MUST not contain these
    ///       symbols at start. However for this case we make path clean via Trim. ROOT dir could be passes as empty string
    /// </summary>
    public class AWSCompatibleCloudFileStorageManager : IAWSCompatibleFileStorageManager
    {
        public AWSCompatibleCloudFileStorageManager(IDictionary<string, S3StorageSettings> sources, ILoggerFactory loggerFactory)
        {
            // todo(UMV): add && use TokenCancellationSource
            _sources = sources;
            _logger = loggerFactory.CreateLogger<AWSCompatibleCloudFileStorageManager>();
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
                    _logger.LogError( $"Incorrect S3 Storage configuration: {e.Message} , ensure S3Storage section has correct values");
                    _logger.LogDebug(e.ToString());
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
                string msg = $"An error occurred during getting buckets list: {e.Message}";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<IList<string>>(false, (int)HttpStatusCode.InternalServerError, msg, null);
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
                bool isRoot = path.Length == 0 || (path.Length <= 2 && (path[0] == '.' || path[0] == '/'));
                string realPath = !isRoot ? path.TrimStart(new []{'.'}).Trim(new []{'/'}) : path;

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
                        string objKey = s3Object.Key.TrimEnd(new[]{'/'});
                        // 1. Filter by path, Key is a file name with path to file
                        if (!isRoot)
                        {
                            if (s3Object.Key.StartsWith(realPath) && objKey != realPath)
                            {
                                // here we should clarify one thing
                                string[] partsOfPath = realPath.Split("/");
                                string[] parts = objKey.Split("/");
                                if (parts.Length - partsOfPath.Length <= 1)
                                {
                                    objs.Add(new TinyFileInfo(parts.Last(), objKey,
                                        !s3Object.Size.HasValue || s3Object.Size == 0,
                                        s3Object.Size ?? 0));
                                }
                            }
                        }
                        else
                        {
                            if (!objKey.Contains("/"))
                            {
                                objs.Add(new TinyFileInfo(objKey, objKey, !s3Object.Size.HasValue || s3Object.Size == 0, 
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
        /// <param name="additionalParams">Additional params contains Bucket as a param</param>
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
                byte[] bytes;
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
                return new OperationResultDto<MemoryStream>(false, (int) HttpStatusCode.InternalServerError, msg, null);
            }
        }

        /// <summary>
        ///     Creates directory in S3 cloud storage
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="path">Path to directory if dir should be created in root left blank, shouldn't started from ./ or / </param>
        /// <param name="dirName">Name of creating directory</param>
        /// <param name="additionalParams">Additional params contains Bucket as a param</param>
        /// <returns></returns>
        public async Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName,
            IDictionary<string, string>additionalParams)
        {
            string bucket = "";
            try
            {
                IAmazonS3 s3Client = _s3Clients[source];
                bucket = additionalParams[BucketParam];
                string key = Path.Combine(path, dirName);
                key = PrepareS3AWSCompatibleKey(key, true);

                Tuple<bool, string> result = await CreateObjectImpl(s3Client, bucket, key, null);
                int statusCode = result.Item1 ? (int) HttpStatusCode.OK : (int) HttpStatusCode.InternalServerError;
                string outputKey = result.Item1 ? key : String.Empty;
                return new OperationResultDto<string>(result.Item1, statusCode, result.Item2, outputKey);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during directory create in bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError, msg, String.Empty);
            }
        }

        /// <summary>
        ///     Delete directory from S3 Cloud storage
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="dirPath">path (key) of directory</param>
        /// <param name="additionalParams">Additional params contains Bucket as a param</param>
        /// <returns></returns>
        public async Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath, 
            IDictionary<string, string>additionalParams)
        {
            string bucket = "";
            try
            {
                IAmazonS3 s3Client = _s3Clients[source];
                bucket = additionalParams[BucketParam];
                string key = PrepareS3AWSCompatibleKey(dirPath, true);
                Tuple<bool, string> result = await DeleteObjectImpl(s3Client, bucket, key);
                int statusCode = result.Item1 ? (int) HttpStatusCode.OK : (int) HttpStatusCode.InternalServerError;
                return new OperationResultDto<bool>(result.Item1, statusCode, String.Empty, result.Item1);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during directory delete from bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
            }
        }

        /// <summary>
        ///     Creates file in S3 Cloud storage
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="path">Path to directory if dir should be created in root left blank, shouldn't started from ./ or /</param>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public async Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent,
            IDictionary<string, string>additionalParams)
        {
            string bucket = "";
            try
            {
                IAmazonS3 s3Client = _s3Clients[source];
                bucket = additionalParams[BucketParam];
                string key = Path.Combine(path, fileName);
                key = PrepareS3AWSCompatibleKey(key, false);
                Tuple<bool, string> result = await CreateObjectImpl(s3Client, bucket, key, fileContent);
                int statusCode = result.Item1 ? (int) HttpStatusCode.OK : (int) HttpStatusCode.InternalServerError;
                string outputKey = result.Item1 ? key : String.Empty;
                return new OperationResultDto<string>(result.Item1, statusCode, result.Item2, outputKey);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during file create in bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError, msg, String.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="filePath"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public async Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath, IDictionary<string, string>additionalParams)
        {
            string bucket = "";
            try
            {
                IAmazonS3 s3Client = _s3Clients[source];
                bucket = additionalParams[BucketParam];
                string key = PrepareS3AWSCompatibleKey(filePath, false);
                Tuple<bool, string> result = await DeleteObjectImpl(s3Client, bucket, key);
                int statusCode = result.Item1 ? (int) HttpStatusCode.OK : (int) HttpStatusCode.InternalServerError;
                return new OperationResultDto<bool>(result.Item1, statusCode, String.Empty, result.Item1);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during file delete from bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Cloud S3 service identifier</param>
        /// <param name="filePath"></param>
        /// <param name="fileContent"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public async Task<OperationResultDto<bool>> UpdateFileAsync(string source, string filePath, MemoryStream fileContent, 
            IDictionary<string, string>additionalParams)
        {
            string bucket = "";
            try
            {
                IAmazonS3 s3Client = _s3Clients[source];
                bucket = additionalParams[BucketParam];
                string key = PrepareS3AWSCompatibleKey(filePath, false);
                Tuple<bool,string> rmResult = await DeleteObjectImpl(s3Client, bucket, key);
                if (!rmResult.Item1)
                {
                    return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, rmResult.Item2, false);
                }

                Tuple<bool, string> crResult = await CreateObjectImpl(s3Client, bucket, key, fileContent);
                int statusCode = crResult.Item1 ? (int) HttpStatusCode.OK : (int) HttpStatusCode.InternalServerError;
                return new OperationResultDto<bool>(crResult.Item1, statusCode, crResult.Item2, crResult.Item1);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during file update: \"{filePath}\" from bucket: \"{bucket}\", error:\"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
            }
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

        /// <summary>
        ///     This method using either for creating files and folders, the oly difference is that File has some
        ///     data, therefore objData is not null, for Folder is Null 
        /// </summary>
        /// <param name="s3Client"></param>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="objData"></param>
        /// <returns></returns>
        private async Task<Tuple<bool, string>> CreateObjectImpl(IAmazonS3 s3Client, string bucket, string key, MemoryStream objData)
        {
            try
            {
                PutObjectRequest request = new PutObjectRequest
                {
                    InputStream = objData,
                    BucketName = bucket,
                    Key = key,
                    DisablePayloadSigning = true,
                };

                PutObjectResponse response = await s3Client.PutObjectAsync(request);
                bool result = response.HttpStatusCode == HttpStatusCode.OK;
                return new Tuple<bool, string>(result, string.Empty);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during object creation with key: \"{key}\" to bucket: \"{bucket}\", error:\"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new Tuple<bool, string>(false, msg);
            }
        }

        private async Task<Tuple<bool, string>> DeleteObjectImpl(IAmazonS3 s3Client, string bucket, string key)
        {
            try
            {
                DeleteObjectResponse response = await s3Client.DeleteObjectAsync(bucket, key);
                bool result = response.HttpStatusCode == HttpStatusCode.NoContent;
                return new Tuple<bool, string>(result, String.Empty);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during object delete with key: \"{key}\" from bucket: \"{bucket}\", error:\"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new Tuple<bool, string>(false, msg);
            }
        }
        
        private string PrepareS3AWSCompatibleKey(string path, bool isDirectory)
        {
            string key = path.TrimStart(new[] {'.'}).TrimStart(new[] {'/'}).Replace("\\", "/");;
            if (isDirectory && !key.EndsWith("/"))
                key += "/";
            return key;
        }

        public event DirectorySuccessfullyCreatedHandler OnDirectoryCreated;
        public event DirectorySuccessfullyDeletedHandler OnDirectoryDeleted;
        public event FileSuccessfullyCreatedHandler OnFileCreated;
        public event FileSuccessfullyDeletedHandler OnFileDeleted;

        public const string BucketParam = "bucket";

        private readonly IDictionary<string, S3StorageSettings> _sources = new Dictionary<string, S3StorageSettings>();
        private readonly IDictionary<string, IAmazonS3> _s3Clients = new ConcurrentDictionary<string, IAmazonS3>();
        private readonly ILogger<AWSCompatibleCloudFileStorageManager> _logger;
    }
}