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
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Minio.S3.Settings;

namespace Wissance.WebApiToolkit.Minio.S3.Managers
{
    public class MinioFileStorageManager : IMinioFileStorageManager
    {
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

        public void Dispose()
        {
            foreach (KeyValuePair<string,IMinioClient> client in _clients)
            {
                client.Value.Dispose();
            }
        }
        
        public OperationResultDto<IList<string>> GetSources()
        {
            return new OperationResultDto<IList<string>>(true, (int)HttpStatusCode.OK, String.Empty, 
                _sources.Keys.ToList());
        }

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

        public Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath, IDictionary<string, string> additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName, IDictionary<string, string> additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath, IDictionary<string, string> additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent,
            IDictionary<string, string> additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath, IDictionary<string, string> additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> UpdateFileAsync(string source, string filePath, MemoryStream fileContent, IDictionary<string, string> additionalParams = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<IList<string>>> GetBucketsAsync(string source)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> CreateBucketAsync(string source, string bucketName)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> DeleteBucketAsync(string source, string bucketName)
        {
            throw new System.NotImplementedException();
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