using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Core.Events;
using Wissance.WebApiToolkit.Core.Globals;
using Wissance.WebApiToolkit.Core.Managers.Helpers;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Managers
{
    /// <summary>
    ///    Manager implementation of IFileManager that allows to work with files that are available as
    ///    filesystem om server machine.
    ///    Files are not separated from some logic, i.e. in database. There are some ways of achieving
    ///    this :
    ///        1. Add service and subscribe on events i.e. at controller level
    ///        2. Decorate method call via class from this class (WebFolderFileManager)
    ///           and do following i.e. :
    ///           CreateFileAsync()
    ///           {
    ///               // 1. do before file creation
    ///               var res = await base.CreateFileAsync();
    ///               // 2. do after file creation
    ///           }
    ///        3. Via super manager class that have 2 or more manager classes i.e. one for db, one - WebFolderFileManager
    /// </summary>
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

        /// <summary>
        ///     Returns a list of files or directories in the source in the specified directory (path).
        ///     Does not return a list of files in subdirectories; should be queried separately.
        /// </summary>
        /// <param name="source">source identifier</param>
        /// <param name="path">directory in the source to get list of a files and dirs</param>
        /// <returns>List of FileInfo with a tiny info about items (name, file or dir, and size if file) </returns>
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
                _logger.LogError($"An error occurred during \"GetFiles\" method call for source: \"{source}\" for path: \"{path}\", error: {e.Message}");
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<IList<TinyFileInfo>>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetUnknownErrorMessage("GetFiles", e.Message), null);
            }
        }

        /// <summary>
        ///    Reads file content in a source and returns it content as a MemoryStream wrapped in OperationResultDto
        /// </summary>
        /// <param name="source">source identifier</param>
        /// <param name="filePath">path to file</param>
        /// <returns>File binary content as a MemoryStream</returns>
        public async Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath)
        {
            try
            {
                if (!_sources.ContainsKey(source))
                    return new OperationResultDto<MemoryStream>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetBadSourceErrorMessage(source), null);
                string fullFilePath = Path.GetFullPath(Path.Combine(_sources[source], filePath));
                if (!File.Exists(fullFilePath))
                {
                    return new OperationResultDto<MemoryStream>(false, (int) HttpStatusCode.BadRequest,
                        ResponseMessageBuilder.GetBadRequestErrorMessage(MessageCatalog.FileResourceType, PathProperty, filePath), null);
                }

                FileStream fStream = new FileStream(fullFilePath, FileMode.Open);
                MemoryStream mStream = new MemoryStream();
                await fStream.CopyToAsync(mStream);
                return new OperationResultDto<MemoryStream>(true, (int) HttpStatusCode.OK, String.Empty, mStream);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during \"GetFileContent\" method call for source: \"{source}\" for file: \"{filePath}\", error: {e.Message}");
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<MemoryStream>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetUnknownErrorMessage("GetFileContent", e.Message), null);
            }
        }

        /// <summary>
        ///     Creates a new directory with specified dirName in a source relative to a path. If directory already
        ///     exists returns error (Conflict)
        /// </summary>
        /// <param name="source">source identifier</param>
        /// <param name="path">folder relative to source</param>
        /// <param name="dirName">name of creating directory</param>
        /// <returns>Path to created directory wrapped in a OperationResultDto</returns>
        public async Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName)
        {
            string dirPath = "";
            try
            {
                if (!_sources.ContainsKey(source))
                    return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetBadSourceErrorMessage(source), null);
                dirPath = Path.GetFullPath(Path.Combine(_sources[source], path, dirName));
                if (Directory.Exists(dirPath))
                {
                    return new OperationResultDto<string>(false, (int) HttpStatusCode.Conflict,
                        ResponseMessageBuilder.GetConflictErrorMessage(MessageCatalog.DirectoryResourceType, PathProperty, dirPath), null);
                }

                DirectoryInfo info = Directory.CreateDirectory(dirPath);
                OnDirectoryCreated?.Invoke(this, new DirectorySuccessfullyCreatedEventArgs(source, dirPath));
                return new OperationResultDto<string>(true, (int) HttpStatusCode.Created, String.Empty, dirPath);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during \"CreateDir\" method call for source: \"{source}\" for directory: \"{dirPath}\", error: {e.Message}");
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetUnknownErrorMessage("CreateDir", e.Message), null);
            }
        }

        /// <summary>
        ///     Removes directory recursively with all files and subdirectories, if directory does not exists
        ///     returns false in success (NotFound)
        /// </summary>
        /// <param name="source">source identifier</param>
        /// <param name="dirPath">path of removing directory</param>
        /// <returns> bool as a result of directory delete</returns>
        public async Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath)
        {
            try
            {
                if (!_sources.ContainsKey(source))
                    return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetBadSourceErrorMessage(source), false);
                string fullDirPath = Path.GetFullPath(Path.Combine(_sources[source], dirPath));
                if (!Directory.Exists(fullDirPath))
                {
                    return new OperationResultDto<bool>(false, (int) HttpStatusCode.NotFound,
                        ResponseMessageBuilder.GetResourceNotFoundMessage("Directory", fullDirPath), false);
                }

                Directory.Delete(dirPath, true);
                OnDirectoryDeleted?.Invoke(this, new DirectorySuccessfullyDeletedEventArgs());
                return new OperationResultDto<bool>(true, (int) HttpStatusCode.NoContent,
                    ResponseMessageBuilder.GetResourceNotFoundMessage("Directory", fullDirPath), true);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during \"DeleteDir\" method call for source: \"{source}\" for directory: \"{dirPath}\", error: {e.Message}");
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetUnknownErrorMessage("DeleteDir", e.Message), false);
            }
        }

        /// <summary>
        ///     Creates a new file in a source, if file with such name already exists returns error (Conflict).
        /// </summary>
        /// <param name="source">source identifier</param>
        /// <param name="path">path where file should be created</param>
        /// <param name="fileName">name of a newly creating file</param>
        /// <param name="fileContent">binary file content</param>
        /// <returns>Path to a newly created file wrapped in OperationResultDto</returns>
        public async Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, 
            MemoryStream fileContent)
        {
            string fullFileName = "";
            try
            {
                if (!_sources.ContainsKey(source))
                    return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetBadSourceErrorMessage(source), null);
                fullFileName = Path.Combine(_sources[source], path, fileName);
                if (File.Exists(fullFileName))
                {
                    return new OperationResultDto<string>(false, (int) HttpStatusCode.Conflict,
                        ResponseMessageBuilder.GetConflictErrorMessage(MessageCatalog.FileResourceType, PathProperty, fullFileName), null);
                }

                await File.WriteAllBytesAsync(fullFileName, fileContent.GetBuffer());
                OnFileCreated?.Invoke(this, new FileSuccessfullyCreatedEventArgs(source, fullFileName));

                return new OperationResultDto<string>(true, (int) HttpStatusCode.Created, String.Empty, fullFileName);
            }
            catch (Exception e)
            {
                _logger.LogDebug(e.ToString());
                _logger.LogError($"An error occurred during \"CreateFile\" method call for source: \"{source}\" for file: \"{fullFileName}\", error: {e.Message}");
                return new OperationResultDto<string>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetUnknownErrorMessage("CreateFile", e.Message), null);
            }
        }

        /// <summary>
        ///     Removes file in source. Returns error if file does not exists (NotFound). 
        /// </summary>
        /// <param name="source"> source identifier </param>
        /// <param name="filePath">path to file</param>
        /// <returns>bool as a result of file delete operation</returns>
        public async Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath)
        {
            string fullFileName = "";
            try
            {
                if (!_sources.ContainsKey(source))
                    return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetBadSourceErrorMessage(source), false);
                fullFileName = Path.GetFullPath(Path.Combine(_sources[source], filePath));
                if (!File.Exists(fullFileName))
                {
                    return new OperationResultDto<bool>(false, (int) HttpStatusCode.NotFound,
                        ResponseMessageBuilder.GetResourceNotFoundMessage(MessageCatalog.FileResourceType, filePath), false);
                }
                File.Delete(fullFileName);
                OnFileDeleted?.Invoke(this, new FileSuccessfullyDeletedEventArgs());
                return new OperationResultDto<bool>(true, (int) HttpStatusCode.NoContent, String.Empty, true);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during \"DeleteFile\" method call for source: \"{source}\" for file: \"{fullFileName}\", error: {e.Message}");
                _logger.LogDebug(e.ToString());
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError,
                    ResponseMessageBuilder.GetUnknownErrorMessage("DeleteFile", e.Message), false);
            }
        }

        public event DirectorySuccessfullyCreatedHandler OnDirectoryCreated;
        public event DirectorySuccessfullyDeletedHandler OnDirectoryDeleted;
        public event FileSuccessfullyCreatedHandler OnFileCreated;
        public event FileSuccessfullyDeletedHandler OnFileDeleted;

        private const string PathProperty = "path";

        private readonly IDictionary<string, string> _sources;
        private readonly ILogger<WebFolderFileManager> _logger;
    }
}