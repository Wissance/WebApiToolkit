using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.DataModel.Result;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Minio.S3.Settings;

namespace Wissance.WebApiToolkit.Minio.S3.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class MinioFileStorageManager : IMinioFileStorageManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="loggerFactory"></param>
        public MinioFileStorageManager(IDictionary<string, MinioSettings> sources, ILoggerFactory loggerFactory)
        {
            _sources = sources;
            _logger = loggerFactory.CreateLogger<MinioFileStorageManager>();
            foreach (KeyValuePair<string, MinioSettings> source in sources)
            {
                try
                {
                    IMinioClient client = new MinioClient().WithEndpoint(source.Value.Endpoint)
                                                           .WithCredentials(source.Value.AccessKey, 
                                                                            source.Value.SecretAccessKey);
                    _clients.Add(source.Key, client);
                }
                catch (Exception e)
                {
                    _logger.LogError( $"Incorrect Minio Storage configuration: {e.Message} or there are some other issues");
                    _logger.LogDebug(e.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string,IMinioClient> client in _clients)
            {
                client.Value.Dispose();
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async Task<OperationResultDto<IList<string>>> GetBucketsAsync(string source)
        {
            try
            {
                IMinioClient client = _clients[source];
                ListAllMyBucketsResult result = await client.ListBucketsAsync();
                return new OperationResultDto<IList<string>>(true, (int) HttpStatusCode.OK, String.Empty,
                    result.Buckets.Select(b => b.Name).ToList());
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
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<OperationResultDto<bool>> CreateBucketAsync(string source, string bucketName)
        {
            try
            {
                IMinioClient client = _clients[source];
                await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                return new OperationResultDto<bool>(true, (int) HttpStatusCode.OK, String.Empty, true);
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
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<OperationResultDto<bool>> DeleteBucketAsync(string source, string bucketName)
        {
            try
            {
                IMinioClient client = _clients[source];
                await client.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(bucketName));
                return new OperationResultDto<bool>(true, (int) HttpStatusCode.NoContent, String.Empty, true);
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
        /// 
        /// </summary>
        /// <returns></returns>
        public OperationResultDto<IList<string>> GetSources()
        {
            return new OperationResultDto<IList<string>>(true, (int)HttpStatusCode.OK, String.Empty, 
                _sources.Keys.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public async Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync(string source, string path = ".", IDictionary<string, string> additionalParams = null)
        {
            string bucket = "";
            try
            {
                IMinioClient client = _clients[source];
                bucket = additionalParams[BucketParam];
                ListObjectsArgs listObjects = new ListObjectsArgs();
                IList<TinyFileInfo> objects = new List<TinyFileInfo>();
                await foreach (Item obj in client.ListObjectsEnumAsync(listObjects.WithPrefix(path).WithBucket(bucket)))
                {
                    // get file name from obj.Key
                    string[] parts = obj.Key.Split(new[] {'/'});
                    // if file is Directory therefore it has name ending with a slash i.e. tmp -> tmp/
                    int index = parts.Length >= 2 ? parts.Length - 2 : 0;
                    string name = obj.IsDir ? parts.Last() : parts[index];
                    TinyFileInfo info = new TinyFileInfo(name, obj.Key, obj.IsDir, (long)obj.Size);
                    objects.Add(info);
                }

                return new OperationResultDto<IList<TinyFileInfo>>(true, (int) HttpStatusCode.OK, String.Empty,
                    objects);
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
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filePath"></param>
        /// <param name="additionalParams"></param>
        /// <returns></returns>
        public async Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath, IDictionary<string, string> additionalParams = null)
        {
            string bucket = "";
            try
            {
                IMinioClient client = _clients[source];
                bucket = additionalParams[BucketParam];
                MemoryStream memoryStream = new MemoryStream();
                await client.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(filePath)
                    .WithCallbackStream(async stream => await stream.CopyToAsync(memoryStream))
                );
                return new OperationResultDto<MemoryStream>(true, (int) HttpStatusCode.OK, String.Empty, memoryStream);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during getting file content from bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<MemoryStream>(false, (int) HttpStatusCode.InternalServerError, msg, null);
            }
        }

        public async Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName, 
            IDictionary<string, string> additionalParams = null)
        {
            string bucket = "";
            try
            {
                IMinioClient client = _clients[source];
                bucket = additionalParams[BucketParam];
                string key = Path.Combine(path, dirName);
                key = PrepareS3CompatibleKey(key, true);
                return await CreateObjectImpl(client, bucket, key, null);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during directory create in bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError, msg, String.Empty);
            }
        }

        public async Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath, IDictionary<string, string> additionalParams = null)
        {
            string bucket = "";
            try
            {
                IMinioClient client = _clients[source];
                bucket = additionalParams[BucketParam];
                string key = PrepareS3CompatibleKey(dirPath, true);
                Tuple<bool, string> result = await DeleteObjectImpl(client, bucket, key);
                int statusCode = result.Item1 ? (int) HttpStatusCode.NoContent : (int) HttpStatusCode.InternalServerError;
                return new OperationResultDto<bool>(result.Item1, statusCode, result.Item2,result.Item1);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during directory delete from bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
            }
        }

        public async Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent,
            IDictionary<string, string> additionalParams = null)
        {
            string bucket = "";
            try
            {
                IMinioClient client = _clients[source];
                bucket = additionalParams[BucketParam];
                string key = Path.Combine(path, fileName);
                key = PrepareS3CompatibleKey(key, false);
                return await CreateObjectImpl(client, bucket, key, fileContent);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during file create in bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError, msg, String.Empty);
            }
        }

        public async Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath, IDictionary<string, string> additionalParams = null)
        {
            string bucket = "";
            try
            {
                IMinioClient client = _clients[source];
                bucket = additionalParams[BucketParam];
                string key = PrepareS3CompatibleKey(filePath, false);
                Tuple<bool, string> result = await DeleteObjectImpl(client, bucket, key);
                int statusCode = result.Item1 ? (int) HttpStatusCode.NoContent : (int) HttpStatusCode.InternalServerError;
                return new OperationResultDto<bool>(result.Item1, statusCode, result.Item2,result.Item1);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during file delete from bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
            }
        }

        public async Task<OperationResultDto<bool>> UpdateFileAsync(string source, string filePath, MemoryStream fileContent, IDictionary<string, string> additionalParams = null)
        {
            string bucket = "";
            try
            {
                IMinioClient client = _clients[source];
                bucket = additionalParams[BucketParam];
                string key = PrepareS3CompatibleKey(filePath, false);
                Tuple<bool,string> rmResult = await DeleteObjectImpl(client, bucket, key);
                if (!rmResult.Item1)
                {
                    return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, rmResult.Item2, false);
                }

                OperationResultDto<string> crResult = await CreateObjectImpl(client, bucket, key, fileContent);
                return new OperationResultDto<bool>(crResult.Success, crResult.Status, crResult.Message, crResult.Success);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during file update: \"{filePath}\" from bucket: \"{bucket}\", error:\"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
            }
        }

        private async Task<OperationResultDto<string>> CreateObjectImpl(IMinioClient client, string bucket, string key,
            MemoryStream objData)
        {
            try
            {
                long len = objData?.Length ?? 0;
                PutObjectResponse response = await client.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
                    .WithStreamData(objData)
                    .WithObjectSize(len));
                bool success = response.ResponseStatusCode == HttpStatusCode.Created;
                return new OperationResultDto<string>(success, (int)response.ResponseStatusCode, String.Empty, response.ObjectName);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during object create in bucket with name \"{bucket}\", error: \"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError, msg, String.Empty);
            }
        }

        private async Task<Tuple<bool, string>> DeleteObjectImpl(IMinioClient client, string bucket, string key)
        {
            try
            {
                await client.RemoveObjectAsync(new RemoveObjectArgs().WithObject(key).WithBucket(bucket));
                return new Tuple<bool, string>(true, String.Empty);
            }
            catch (Exception e)
            {
                string msg = $"An error occurred during object delete with key: \"{key}\" from bucket: \"{bucket}\", error:\"{e.Message}\"";
                _logger.LogError(msg);
                _logger.LogDebug(e.ToString());
                return new Tuple<bool, string>(false, msg);
            }
        }

        private string PrepareS3CompatibleKey(string path, bool isDirectory)
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

        private readonly IDictionary<string, MinioSettings> _sources = new Dictionary<string, MinioSettings>();
        private readonly IDictionary<string, IMinioClient> _clients = new ConcurrentDictionary<string, IMinioClient>();
        
        private readonly ILogger<MinioFileStorageManager> _logger;
    }
}