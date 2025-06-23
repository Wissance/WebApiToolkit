using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Managers
{
    public interface IFileManager
    {
        /// <summary>
        ///    Returns list of items files or folders from specified source (webFolder id or S3 Bucket name).
        ///    In case of FileSystem source probably is a directory that is setting up during the application
        ///    startup. In case of S3 implementation we could dynamically Create && Delete Buckets therefore
        ///    S3 implementation is more complicated
        /// </summary>
        /// <param name="source">source identifier</param>
        /// <param name="path">relative path inside source</param>
        /// <returns></returns>
        Task<OperationResultDto<IList<TinyFileInfo>>> GetFiles(string source, string path);
        // todo(UMV): add GetFileContent()
        /// <summary>
        ///     This method creates directory in specified source relative to path
        /// </summary>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <param name="dirName"></param>
        /// <returns></returns>
        Task<OperationResultDto<bool>> CreateDir(string source, string path, string dirName);
        Task<OperationResultDto<bool>> RemoveDir(string source, string path, string dirName);
        Task<OperationResultDto<bool>> CreateFile(string source, string path, IFormFile file);
        Task<OperationResultDto<bool>> RemoveFile(string source, string path, string fileName);
    }
}