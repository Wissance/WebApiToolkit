using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.Data;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Managers.Helpers;

namespace Wissance.WebApiToolkit.Managers
{
    /// <summary>
    ///    This is a Model Manager for working with EntityFramework ORM (EF) as a tool for perform CRUD operations over persistent objects
    ///    It has a default implementation of the following method of IModelManager:
    ///    * GetAsync method for obtain many items
    ///    * GetByIdAsync method for obtain one item by id
    ///    * Delete method 
    /// </summary>
    /// <typeparam name="TObj">Model class implements IModelIdentifiable</typeparam>
    /// <typeparam name="TRes">DTO class (representation of Model in other systems i.e. in frontend))</typeparam>
    /// <typeparam name="TId">Identifier type that is using as database table PK</typeparam>
    public abstract class EfModelManager <TObj, TRes, TId> : IModelManager<TRes, TObj, TId>
                                                where TObj: class, IModelIdentifiable<TId>
                                                where TRes: class
                                                where TId: IComparable
             
    {
        /// <summary>
        ///    Constructor of default model manager requires that Model Context derives from EfDbContext
        /// </summary>
        /// <param name="dbContext">Ef Database context</param>
        /// <param name="createFunc">Delegate (factory func) for creating DTO from Model</param>
        /// <param name="filterFunc">Function that use dictionary with query params to filter result set</param>
        /// <param name="loggerFactory">Logger factory</param>
        /// <exception cref="ArgumentNullException"></exception>
        public EfModelManager(DbContext dbContext, Func<TObj, IDictionary<string, string>, bool> filterFunc, Func<TObj, TRes> createFunc,
                              ILoggerFactory loggerFactory)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
            _logger = loggerFactory.CreateLogger<EfModelManager<TObj, TRes, TId>>();
            _defaultCreateFunc = createFunc;
            _filterFunc = filterFunc;
        }

        /// <summary>
        ///  GetManyAsync method allow to extract data from Ef using optional sorting and filtering (sorting applies first, then filter).
        ///  This method contains more params then GetAsync(), GetAsync default impl calls this method and pass null as filter && sort.
        ///  This method designed for having ADDITIONAL GET methods in controller.
        /// </summary>
        /// <param name="page">Page number starting from 1</param>
        /// <param name="size">Data portion size</param>
        /// <param name="parameters">Query parameters for data filter</param>
        /// <param name="sorting">sorting params (Sort - Field name, Order - Sort direction (ASC, DESC))</param>
        /// <param name="filterFunc">Function that describes how to filter data prior to get a portion</param>
        /// <param name="sortFunc">>Function that describes how to sort data prior to get a portion</param>
        /// <param name="createFunc">Function that describes how to construct DTO from Model, if null passes here then uses _defaultCreateFunc</param>
        /// <returns>OperationResult with data portion</returns>
        public virtual async Task<OperationResultDto<Tuple<IList<TRes>, long>>> GetManyAsync<TF>(int page, int size, IDictionary<string, string> parameters, SortOption sorting,
                                                                                                 Func<TObj, IDictionary<string, string>, bool> filterFunc = null, 
                                                                                                 Func<TObj, TF> sortFunc = null, Func<TObj, TRes> createFunc = null)
        {
            try
            {
                //IQueryable<TObj> filteredObjects = dbSet;
                long totalItems = 0;
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                IList<TObj> entities = null;
                if (sorting != null)
                {
                    if (sortFunc != null)
                    {
                        if (sorting.Order == SortOrder.Ascending)
                        {
                            entities = dbSet.OrderBy(sortFunc).ToList();
                        }
                        else
                        {
                            entities = dbSet.OrderByDescending(sortFunc).ToList();
                        }
                    }
                }

                if (filterFunc != null)
                {
                    // filteredObjects = await dbSet.ToListAsync();
                    if (entities == null)
                    {
                        entities = await dbSet.ToListAsync();
                    }

                    IEnumerable<TObj> items = entities.Where(o => filterFunc(o, parameters));
                    totalItems = items.Count();
                    entities = items.Skip(size * (page - 1)).Take(size).ToList();
                }
                else
                {
                    if (entities == null)
                    {
                        totalItems = await dbSet.LongCountAsync();
                        entities = await dbSet.Skip(size * (page - 1)).Take(size).ToListAsync();
                    }
                    else
                    {
                        totalItems = await dbSet.LongCountAsync();
                        entities = entities.Skip(size * (page - 1)).Take(size).ToList();
                    }
                }
                
                return new OperationResultDto<Tuple<IList<TRes>, long>>(true, (int)HttpStatusCode.OK, null,
                    new Tuple<IList<TRes>, long>(entities.Select(e => createFunc!=null ? createFunc(e) : _defaultCreateFunc(e)).ToList(), totalItems));
            }
            catch (Exception e)
            {
                _logger.LogError($"An error: {e.Message} occurred during collection of object of type: {typeof(TObj)} retrieve and convert to objects of type: {typeof(TRes)}");
                return new OperationResultDto<Tuple<IList<TRes>, long>>(true, (int)HttpStatusCode.InternalServerError, "Error occurred, contact system maintainer",
                    new Tuple<IList<TRes>, long>(null, 0));
            }
        }

        /// <summary>
        /// GetOneAsync method used 4 getting ONE object by id from Database using EF. Optionally could be used different createFunc, but in most
        /// cases (99.(9) percents) this method is identical to GetByIdAsync.
        /// </summary>
        /// <param name="id">item identifier</param>
        /// <param name="createFunc">Function that describes how to construct DTO from Model, if null passes here then uses _defaultCreateFunc</param>
        /// <returns></returns>
        public virtual async Task<OperationResultDto<TRes>> GetOneAsync(TId id, Func<TObj, TRes> createFunc = null)
        {
            try
            {
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                TObj entity = await dbSet.FirstOrDefaultAsync(i => i.Id.Equals(id));
                if (entity == null)
                    return new OperationResultDto<TRes>(false, (int)HttpStatusCode.NotFound, 
                                                        ResponseMessageBuilder.GetResourceNotFoundMessage(typeof(TObj).ToString(), id), null);
                return new OperationResultDto<TRes>(true, (int)HttpStatusCode.OK, null, 
                    createFunc != null?createFunc(entity): _defaultCreateFunc(entity));
            }
            catch (Exception e)
            {
                _logger.LogError($"An error: {e.Message} occurred during object of type: {typeof(TObj)} with id: {id} retrieve and convert to object of type: {typeof(TRes)}");
                return new OperationResultDto<TRes>(false, (int)HttpStatusCode.NotFound,
                                                    ResponseMessageBuilder.GetResourceNotFoundMessage(typeof(TObj).ToString(), id), null);
            }
        }

        /// <summary>
        /// GetAsync return portion of DTO unlike GetMany methods have not a default sorting && filtering . Default implementation
        /// of IModelManager for get data portion via EF.
        /// </summary>
        /// <param name="page">page number starting from 1</param>
        /// <param name="size">size of data portion</param>
        /// <param name="sorting">sorting params (Sort - Field name, Order - Sort direction (ASC, DESC))</param>
        /// <param name="parameters">raw query parameters</param>
        /// <returns>OperationResult with data portion</returns>
        public virtual async Task<OperationResultDto<Tuple<IList<TRes>, long>>> GetAsync(int page, int size, SortOption sorting = null, 
                                                                                         IDictionary<string, string> parameters = null)
        {
            // this method is using default sorting and order, if specific order or sorting is required please specify it using another GetAsync method
            Func<TObj, object> sortingFunc = null;
            if (sorting != null)
            {
                PropertyInfo[] modelProperties = typeof(TObj).GetProperties();
                MethodInfo prop = modelProperties.FirstOrDefault(p => string.Equals(p.Name.ToLower(), sorting.Sort.ToLower()))?.GetGetMethod();
                if (prop != null)
                {
                    sortingFunc = o => prop.Invoke(o, null);
                }
            }
            
            return await GetManyAsync(page, size, parameters, sorting, _filterFunc, sortingFunc);
        }
        
        /// <summary>
        /// GetByIdAsync returns one item by id, IModelManager default implementation
        /// </summary>
        /// <param name="id">item identifier</param>
        /// <returns>OperationResult with one item</returns>
        public async Task<OperationResultDto<TRes>> GetByIdAsync(TId id)
        {
            return await GetOneAsync(id);
        }
        
        /// <summary>
        /// Method for create new object in database using Ef, in this class still have not a default impl, but will be
        /// </summary>
        /// <param name="data">DTO with Model representation</param>
        /// <returns>DTO of newly created object</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<OperationResultDto<TRes>> CreateAsync(TRes data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method for create new objects in database using Ef, in this class still have not a default impl, but will be
        /// </summary>
        /// <param name="data">Array of DTO with Model representation</param>
        /// <returns>Array of DTO of a newly created objects</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<OperationResultDto<TRes[]>> BulkCreateAsync(TRes[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method for update existing objects using EF, still have not default impl, but will be
        /// </summary>
        /// <param name="id">item identifier</param>
        /// <param name="data">>DTO with Model representation</param>
        /// <returns>DTO of updated object</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method for update existing objects in a database using Ef, in this class still have not a default impl, but will be
        /// </summary>
        /// <param name="data">Array of DTO with Model representation</param>
        /// <returns>Array of DTO of a updated objects</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Task<OperationResultDto<TRes[]>> BulkUpdateAsync(TRes[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// DeleteAsync method for remove object from Database using Ef
        /// </summary>
        /// <param name="id">item identifier</param>
        /// <returns>true if removal was successful, otherwise false</returns>
        public async Task<OperationResultDto<bool>> DeleteAsync(TId id)
        {
            try
            {
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                TObj item = await dbSet.FirstOrDefaultAsync(t => t.Id.Equals(id));

                if (item == null)
                    return new OperationResultDto<bool>(false, (int)HttpStatusCode.NotFound, "Item does not exists", false);
                dbSet.Remove(item);
                await _dbContext.SaveChangesAsync();
                return new OperationResultDto<bool>(true, (int)HttpStatusCode.NoContent, null, true);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during object of type: {nameof(TObj)} with id: {id} remove: {e.Message}");
                return new OperationResultDto<bool>(false, (int)HttpStatusCode.InternalServerError, "Error occurred during object delete, contact system maintainer", false);
            }
        }
        
        /// <summary>
        /// BulkDeleteAsync method for remove object from Database using Ef
        /// </summary>
        /// <param name="objectsIds">item identifiers</param>
        /// <returns>true if removal was successful, otherwise false</returns>
        public virtual Task<OperationResultDto<bool>> BulkDeleteAsync(TId[] objectsIds)
        {
            throw new NotImplementedException();
        }

        private readonly ILogger<EfModelManager<TObj, TRes, TId>> _logger;
        private readonly DbContext _dbContext;
        private readonly Func<TObj, TRes> _defaultCreateFunc;
        private readonly Func<TObj, IDictionary<string, string>, bool> _filterFunc;
    }
}
