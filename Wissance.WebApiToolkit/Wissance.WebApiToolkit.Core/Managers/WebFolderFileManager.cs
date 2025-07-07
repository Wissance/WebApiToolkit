using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Core.Managers.Helpers;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Managers
{
    public class WebFolderFileManager : IFileManager
    {
        /// <summary>
        ///     Instantiation of a new WebFolderFileManager
        /// </summary>
        /// <param name="sources">
        ///     list of sources contains string key and string path to web folder.
        ///     i.e. : {"tmp", "./files/share"}
        /// </param>
        /// <param name="loggerFactory"> Logger factory </param>
        public WebFolderFileManager(IDictionary<string, string> sources, ILoggerFactory loggerFactory)
        {
            _sources = sources;
            _logger = loggerFactory.CreateLogger<WebFolderFileManager>();
        }

        // todo(UMV): add storages via constructor
        public async Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync(string source, string path = "")
        {
            try
            {
                IList<TinyFileInfo> filesInfo = new List<TinyFileInfo>();
                if (!_sources.ContainsKey(source))
                    return new OperationResultDto<IList<TinyFileInfo>>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetBadSourceErrorMessage(source), null);
                string dirPath = Path.Combine(_sources[source], path);
                string[] files = Directory.GetFiles(dirPath);
                foreach (string file in files)
                {
                    FileInfo f = new FileInfo(file);
                    filesInfo.Add(new TinyFileInfo(file, f.Length > 0, f.Length));
                    
                }

                return new OperationResultDto<IList<TinyFileInfo>>(true, (int) HttpStatusCode.OK, String.Empty, filesInfo);

            }
            catch (Exception e)
            {
                return new OperationResultDto<IList<TinyFileInfo>>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetUnknownErrorMessage("GetFiles", e.Message), null);
            }
        }

        public async Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath)
        {
            if (!_sources.ContainsKey(source))
                return new OperationResultDto<MemoryStream>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetBadSourceErrorMessage(source), null);
            throw new System.NotImplementedException();
        }

        public async Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName)
        {
            if (!_sources.ContainsKey(source))
                return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetBadSourceErrorMessage(source), null);
            throw new System.NotImplementedException();
        }

        public async Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath)
        {
            if (!_sources.ContainsKey(source))
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetBadSourceErrorMessage(source), false);
            throw new System.NotImplementedException();
        }

        public async Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent)
        {
            if (!_sources.ContainsKey(source))
                return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetBadSourceErrorMessage(source), null);
            throw new System.NotImplementedException();
        }

        public async Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath)
        {
            if (!_sources.ContainsKey(source))
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetBadSourceErrorMessage(source), false);
            throw new System.NotImplementedException();
        }

        public event DirectorySuccessfullyCreatedHandler OnDirectoryCreated;
        public event DirectorySuccessfullyDeletedHandler OnDirectoryDeleted;
        public event FileSuccessfullyCreatedHandler OnFileCreated;
        public event FileSuccessfullyDeletedHandler OnFileDeleted;

        private readonly IDictionary<string, string> _sources;
        private readonly ILogger<WebFolderFileManager> _logger;
    }
}