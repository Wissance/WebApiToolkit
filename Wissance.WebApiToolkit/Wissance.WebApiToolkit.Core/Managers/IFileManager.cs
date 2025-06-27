using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Managers
{
    /// <summary>
    ///    General file manager interface that could be used for different source types:
    ///      - local file system folders
    ///      - nfs i.e. samba
    ///      - ftp
    ///      - S3 (Cloud like Amazon, Cloudflare) or Local (MinIO)
    ///    Source is an every method parameter, string key that references credentials && access option
    ///    Here we don't expect to work with files > 100 Mb and greater, there should be IStreamManager or
    ///    something like this. One thing that also should be noted : we are plan to implement Controller
    ///    that allows to deal with files and usually files are not exists without some other persistant
    ///    data therefore we are going to implement mechanism that allows to manipulate files and related
    ///    data.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        ///    Returns list of items files or folders from specified source (webFolder id or S3 Bucket name).
        ///    In case of FileSystem source probably is a directory that is setting up during the application
        ///    startup. In case of S3 implementation we could dynamically Create && Delete Buckets therefore
        ///    S3 implementation is more complicated
        /// </summary>
        /// <param name="source">source identifier : web folder id or Bucket name</param>
        /// <param name="path">relative path inside source</param>
        /// <returns></returns>
        Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync(string source, string path = ".");
        /// <summary>
        ///     Returns file content (bytes)
        /// </summary>
        /// <param name="source">source identifier : web folder id or Bucket name</param>
        /// <param name="filePath">full</param>
        /// <returns></returns>
        Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath);
        /// <summary>
        ///     Create directory in the source (web folder or s3 Bucket) by relative
        ///     path with name provided in dirName 
        /// </summary>
        /// <param name="source">source identifier : web folder id or Bucket name</param>
        /// <param name="path">relative path inside source</param>
        /// <param name="dirName">creating directory name</param>
        /// <returns>OperationResultDto with Directory full path inside source, if directory wasn't created - null</returns>
        Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName);
        /// <summary>
        ///     Removes directory in source (if directory is not empty it will be removed with all children)
        /// </summary>
        /// <param name="source">source identifier : web folder id or Bucket name</param>
        /// <param name="dirPath">relative path inside source</param>
        /// <returns>OperationResultDto with bool value: true - directory was removed, false - wasn't</returns>
        Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath);
        /// <summary>
        ///     Creates a new file in source by relative path with specified fileName and content in a MemoryStream
        /// </summary>
        /// <param name="source">source identifier : web folder id or Bucket name</param>
        /// <param name="path">relative path inside source</param>
        /// <param name="fileName">name of a creating file</param>
        /// <param name="fileContent">binary data as a stream</param>
        /// <returns> Operation result with full path inside source </returns>
        Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent);
        /// <summary>
        ///     Removes file from source
        /// </summary>
        /// <param name="source">source identifier : web folder id or Bucket name</param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath);
    }
}