using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Managers
{
    /// <summary>
    ///     This is a common interface to perform CRUD and some other common operation over PERSISTENCE Data.
    ///     General convention are:
    ///     1. Get, Update and Create operates with the SAME DTO
    ///     2. All models (TData) MUST implement IModelIdentifiable Generic interface
    ///     3. All operations are wrapped inside OperationResultDto which contains additional information
    ///        about operation success and message is something goes wrong.
    /// </summary>
    /// <typeparam name="TRes">TRes is a Result parameter which used as input and output for operation (DTO)</typeparam>
    /// <typeparam name="TData">Model type (Class that is mapping to PERSISTENT storage)</typeparam>
    /// <typeparam name="TId">Type of identifier, because IModelIdentifiable is a GENERIC</typeparam>
    public interface IModelManager<TRes, TData, TId>
    {
        /// <summary>
        /// Creates a new item in persistent storage (i.e. Database). To assign DTO to Model you should create a custom
        /// ModelManager either deriving from default implementation or you own
        /// </summary>
        /// <param name="data">DTO that contains data 4 creation. This data should be assigned to new model object </param>
        /// <returns>A result of operation with DTO of created object</returns>
        Task<OperationResultDto<TRes>> CreateAsync(TRes data);
        Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data);
        Task<OperationResultDto<bool>> DeleteAsync(TId id);
        Task<OperationResultDto<Tuple<IList<TRes>, long>>> GetAsync(int page, int size);
        Task<OperationResultDto<TRes>> GetByIdAsync(TId id);
    }
}
