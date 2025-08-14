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
                PutObjectResponse response = await client.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
                    .WithStreamData(null)
                    .WithObjectSize(0));
                bool success = response.ResponseStatusCode == HttpStatusCode.Created;
                return new OperationResultDto<string>(success, (int)response.ResponseStatusCode, String.Empty, response.ObjectName);
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
                await client.RemoveObjectAsync(new RemoveObjectArgs().WithObject(key).WithBucket(bucket));
                return new OperationResultDto<bool>(true, (int)HttpStatusCode.NoContent, String.Empty, true);
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
            throw new System.NotImplementedException();
        }

        public async Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath, IDictionary<string, string> additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResultDto<bool>> UpdateFileAsync(string source, string filePath, MemoryStream fileContent, IDictionary<string, string> additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResultDto<IList<string>>> GetBucketsAsync(string source)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResultDto<bool>> CreateBucketAsync(string source, string bucketName)
        {
            throw new System.NotImplementedException();
        }

        public async Task<OperationResultDto<bool>> DeleteBucketAsync(string source, string bucketName)
        {
            throw new System.NotImplementedException();
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