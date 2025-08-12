using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Minio.S3.Managers
{
    public interface IMinioFileStorageManager : IFileManager, IDisposable
    {
        Task<OperationResultDto<IList<string>>> GetBucketsAsync(string source);
        Task<OperationResultDto<bool>> CreateBucketAsync(string source, string bucketName);
        Task<OperationResultDto<bool>> DeleteBucketAsync(string source, string bucketName);
    }
}