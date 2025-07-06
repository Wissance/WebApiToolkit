using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Managers
{
    public class WebFolderFileManager : IFileManager
    {
        public Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync(string source, string path = ".")
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<MemoryStream>> GetFileContentAsync(string source, string filePath)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<string>> CreateDirAsync(string source, string path, string dirName)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> DeleteDirAsync(string source, string dirPath)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<string>> CreateFileAsync(string source, string path, string fileName, MemoryStream fileContent)
        {
            throw new System.NotImplementedException();
        }

        public Task<OperationResultDto<bool>> DeleteFileAsync(string source, string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}