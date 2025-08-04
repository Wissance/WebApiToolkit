using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Core.Events;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Managers
{

    public delegate void DirectorySuccessfullyCreatedHandler(object sender, DirectorySuccessfullyCreatedEventArgs e);
    public delegate void DirectorySuccessfullyDeletedHandler(object sender, DirectorySuccessfullyDeletedEventArgs e);
    public delegate void FileSuccessfullyCreatedHandler(object sender, FileSuccessfullyCreatedEventArgs e);
    public delegate void FileSuccessfullyDeletedHandler(object sender, FileSuccessfullyDeletedEventArgs e);
    
    /// <summary>
    ///    General file manager interface that could be used for different source types:
    ///      - local file system folders
    ///      - nfs i.e. samba
    ///      - ftp
    ///      - S3 (Cloud like Amazon, Cloudflare) or Local (MinIO)
    ///    Source is an every method parameter, string key that references credentials && access option
    ///    Here we don't expect to work with files > 100 Mb and greater, there should be IStreamManager or
    ///    something like this. One thing that also should be noted : we did not implemented Controller
    ///    that allows to deal with files because files are not exists without some other persistant
    ///    data therefore we are going to implement mechanism that allows to manipulate files and related
    ///    data.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        ///   Returns list of available sources. Source is a place where we could get files
        /// </summary>
        /// <returns></returns>
        OperationResultDto<IList<string>> GetSources();
        /// <summary>
        ///    Returns list of items files or folders from specified source (webFolder id or S3 Bucket name).
        ///    In case of FileSystem source probably is a directory that is setting up during the application
        ///    startup. In case of S3 implementation we could dynamically Create && Delete Buckets therefore
        ///    S3 implementation is more complicated
        /// </summary>
        /// <param name="source">source identifier : web folder id or S3 service</param>
        /// <param name="path">full relative path inside source</param>
        /// <param name="additionalParams">additional non regular params depends on this interface implementation</param>
        /// <returns>list of sources keys without details</returns>
        Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync(string source, string path = ".",
            IDictionary<string, string>additionalParams = null);
        /// <summary>
        ///     Returns file content (bytes)
        /// </summary>
        /// <param name="source">source identifier : web folder id or S3 service</param>
        /// <param name="filePath">full relative file path</param>
        /// <param name="additionalParams">additional non regular params depends on this interface implementation</param>
        /// <returns>list of tiny file info: name+type</returns>
        Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath,
            IDictionary<string, string>additionalParams = null);
        /// <summary>
        ///     Create directory in the source (web folder or s3 Bucket) by relative
        ///     path with name provided in dirName 
        /// </summary>
        /// <param name="source">source identifier : web folder id or S3 service</param>
        /// <param name="path">relative path inside source</param>
        /// <param name="dirName">creating directory name</param>
        /// <param name="additionalParams">additional non regular params depends on this interface implementation</param>
        /// <returns>OperationResultDto with Directory full path inside source, if directory wasn't created - null</returns>
        Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName,
            IDictionary<string, string>additionalParams = null);
        /// <summary>
        ///     Removes directory in source (if directory is not empty it will be removed with all children)
        /// </summary>
        /// <param name="source">source identifier : web folder id or S3 service</param>
        /// <param name="dirPath">relative path inside source</param>
        /// <param name="additionalParams">additional non regular params depends on this interface implementation</param>
        /// <returns>OperationResultDto with bool value: true - directory was removed, false - wasn't</returns>
        Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath, 
            IDictionary<string, string>additionalParams = null);
        /// <summary>
        ///     Creates a new file in source by relative path with specified fileName and content in a MemoryStream
        /// </summary>
        /// <param name="source">source identifier : web folder id or S3 service</param>
        /// <param name="path">relative path inside source</param>
        /// <param name="fileName">name of a creating file</param>
        /// <param name="fileContent">binary data as a stream</param>
        /// <param name="additionalParams">additional non regular params depends on this interface implementation</param>
        /// <returns> Operation result with full path inside source </returns>
        Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, 
            MemoryStream fileContent, IDictionary<string, string>additionalParams = null);
        /// <summary>
        ///     Removes file from source
        /// </summary>
        /// <param name="source">source identifier : web folder id or S3 service</param>
        /// <param name="filePath"></param>
        /// <param name="additionalParams">additional non regular params depends on this interface implementation</param>
        /// <returns></returns>
        Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath,
            IDictionary<string, string>additionalParams = null);
        /// <summary>
        ///     Updates existing file
        /// </summary>
        /// <param name="source">source identifier : web folder id or S3 service</param>
        /// <param name="filePath">path to a file</param>
        /// <param name="fileContent">binary data as a stream</param>
        /// <param name="additionalParams">additional non regular params depends on this interface implementation</param>
        /// <returns></returns>
        Task<OperationResultDto<bool>> UpdateFileAsync(string source, string filePath, MemoryStream fileContent,
            IDictionary<string, string>additionalParams = null);
        /// <summary>
        ///     Event occurs when Directory was successfully created via CreateDirAsync
        /// </summary>
        event DirectorySuccessfullyCreatedHandler OnDirectoryCreated;
        /// <summary>
        ///     Event occurs when Directory was successfully deleted via DeleteDirAsync
        /// </summary>
        event DirectorySuccessfullyDeletedHandler OnDirectoryDeleted;
        /// <summary>
        ///     Event occurs when File was successfully created via CreateFileAsync
        /// </summary>
        event FileSuccessfullyCreatedHandler OnFileCreated;
        /// <summary>
        ///     Event occurs when File was successfully deleted via DeleteFileAsync
        /// </summary>
        event FileSuccessfullyDeletedHandler OnFileDeleted;
    }
}