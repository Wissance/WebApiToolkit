using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wissance.WebApiToolkit.Data;
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
    public interface IModelManager<TRes, TData, TId, TQueryParameters>
        where TQueryParameters : class, new()
    {
        /// <summary>
        /// Creates a new item in persistent storage (i.e. Database). To assign DTO to Model you should create a custom
        /// ModelManager either deriving from default implementation or you own
        /// </summary>
        /// <param name="data">DTO that contains data 4 creation. This data should be assigned to new model object </param>
        /// <returns>A result of operation with DTO of created object</returns>
        Task<OperationResultDto<TRes>> CreateAsync(TRes data);
        /// <summary>
        /// Creates a bunch of new items in persistent storage (i.e. Database). To assign DTO to Model you should create a custom
        /// ModelManager either deriving from default implementation or you own
        /// </summary>
        /// <param name="data">DTO that contains data 4 creation. This data should be assigned to new model object </param>
        /// <returns>A result of operation with a DTO with Array of created objects</returns>
        Task<OperationResultDto<TRes[]>> BulkCreateAsync(TRes[] data);
        /// <summary>
        /// Updates existing item that could be found in persistent storage (i.e. database) by id. This method impl
        /// should include update of only required field and probably couldn't be fully generalized
        /// </summary>
        /// <param name="id">Identifier of object in persistent storage</param>
        /// <param name="data">DTO containing representation ob object in other systems or frontend</param>
        /// <returns>A result of operation with DTO of updated object</returns>
        Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data);
        /// <summary>
        /// Updates existing items that could be found in persistent storage (i.e. database) by id. Id should be passed
        /// in object class This method impl should include update of only required field and probably couldn't be fully
        /// generalized ()
        /// </summary>
        /// <param name="data">DTO containing representation ob object in other systems or frontend</param>
        /// <returns>A result of operation with DTO of updated object</returns>
        Task<OperationResultDto<TRes[]>> BulkUpdateAsync(TRes[] data);
        /// <summary>
        /// Removes object from persistent storage by id
        /// </summary>
        /// <param name="id">identifier of object that should be removed</param>
        /// <returns>true if object was removed successfully, otherwise - false</returns>
        Task<OperationResultDto<bool>> DeleteAsync(TId id);
        /// <summary>
        /// Removes objects from persistent storage by their identifiers
        /// </summary>
        /// <param name="objectsIds">identifiers of objects that should be removed</param>
        /// <returns>true if objects were removed successfully, otherwise - false</returns>
        Task<OperationResultDto<bool>> BulkDeleteAsync(TId[] objectsIds);
        /// <summary>
        /// Return a set of DTO objects representation with paging
        /// </summary>
        /// <param name="page">number of page, starting from 1</param>
        /// <param name="size">size of data potion (size of IList)</param>
        /// <param name="sorting">sorting params (Sort - Field name, Order - sort direction (ASC, DESC) )</param>
        /// <param name="parameters">raw query parameters</param>
        /// <returns></returns>
        Task<OperationResultDto<Tuple<IList<TRes>, long>>> GetAsync(int page, int size, SortOption sorting = null,
                                                                    TQueryParameters parameters = null);
        /// <summary>
        /// Return DTO representation of 1 object 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<OperationResultDto<TRes>> GetByIdAsync(TId id);
    }
}
