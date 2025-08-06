using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wissance.WebApiToolkit.Core.Data;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Core.Managers.Helpers;

namespace Wissance.WebApiToolkit.Ef.Managers
{
    /// <summary>
    ///    This is a Model Manager for working with EntityFramework ORM (EF) as a tool for perform CRUD operations over persistent objects
    ///    It has a default implementation of the following method of IModelManager:
    ///    * GetAsync method for obtain many items
    ///    * GetByIdAsync method for obtain one item by id
    ///    * Delete method 
    /// </summary>
    /// <typeparam name="TRes">DTO class (representation of Model in other systems i.e. in frontend))</typeparam>
    /// <typeparam name="TObj">Model class implements IModelIdentifiable</typeparam>
    /// <typeparam name="TId">Identifier type that is using as database table PK</typeparam>
    public abstract class EfModelManager <TRes, TObj, TId> : IModelManager<TRes, TObj, TId>
                                                where TObj: class, IModelIdentifiable<TId>
                                                where TRes: class
                                                where TId: IComparable
             
    {
        /// <summary>
        ///    Constructor of default model manager requires that Model Context derives from EfDbContext
        /// </summary>
        /// <param name="dbContext">Ef Database context</param>
        /// <param name="createResFunc">Delegate (factory func) for creating DTO from Model</param>
        /// <param name="createObjFunc">Delegate (factory func) for creating Entity from DTO</param>
        /// <param name="updateObjFunc">Delegate (factory func) for updating Entity from DTO</param>
        /// <param name="filterFunc">Function that use dictionary with query params to filter result set</param>
        /// <param name="loggerFactory">Logger factory</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public EfModelManager(DbContext dbContext, Func<TObj, IDictionary<string, string>, bool> filterFunc, 
                              Func<TObj, TRes> createResFunc, Func<TRes, TObj> createObjFunc,
                              Action<TRes, TId, TObj> updateObjFunc,
                              ILoggerFactory loggerFactory)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
            _logger = loggerFactory.CreateLogger<EfModelManager<TRes, TObj, TId>>();
            _defaultCreateResFunc = createResFunc;
            _defaultCreateObjFunc = createObjFunc;
            _defaultUpdateObjFunc = updateObjFunc;
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
                            entities = Enumerable.OrderBy<TObj, TF>(dbSet, sortFunc).ToList();
                        }
                        else
                        {
                            entities = Enumerable.OrderByDescending<TObj, TF>(dbSet, sortFunc).ToList();
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
                        entities = await Queryable.Skip<TObj>(dbSet, size * (page - 1)).Take(size).ToListAsync();
                    }
                    else
                    {
                        totalItems = await dbSet.LongCountAsync();
                        entities = entities.Skip(size * (page - 1)).Take(size).ToList();
                    }
                }
                
                return new OperationResultDto<Tuple<IList<TRes>, long>>(true, (int)HttpStatusCode.OK, null,
                    new Tuple<IList<TRes>, long>(entities.Select(e => createFunc!=null ? createFunc(e) : _defaultCreateResFunc(e)).ToList(), totalItems));
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
                    createFunc != null?createFunc(entity): _defaultCreateResFunc(entity));
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
        ///     Method for create new object in database using Ef, using _create
        /// </summary>
        /// <param name="data">DTO with a Model representation</param>
        /// <returns>DTO of a newly created object</returns>
        public virtual async Task<OperationResultDto<TRes>> CreateAsync(TRes data)
        {
            try
            {
                if (_defaultCreateObjFunc == null)
                {
                    return new OperationResultDto<TRes>(false, (int) HttpStatusCode.NotImplemented, 
                        ResponseMessageBuilder.GetNotImplementedErrorMessage(typeof(TObj).ToString(), "Create"), null);
                }

                TObj entity = _defaultCreateObjFunc(data);
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                await dbSet.AddAsync(entity);
                int saveResult = await _dbContext.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    return new OperationResultDto<TRes>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetUnknownErrorMessage("Create", typeof(TObj).ToString()), null);
                }

                return new OperationResultDto<TRes>(true, (int) HttpStatusCode.Created, String.Empty,
                    _defaultCreateResFunc(entity));
            }
            catch (Exception e)
            {
                string msg = ResponseMessageBuilder.GetCreateFailureMessage(typeof(TObj).ToString(), e.Message);
                _logger.LogError(msg);
                return new OperationResultDto<TRes>(false, (int) HttpStatusCode.InternalServerError, msg, null);
            }
        }

        /// <summary>
        /// Method for create new objects in database using Ef, in this class still have not a default impl, but will be
        /// </summary>
        /// <param name="data">Array of DTO with Model representation</param>
        /// <returns>Array of DTO of a newly created objects</returns>
        public virtual async Task<OperationResultDto<TRes[]>> BulkCreateAsync(TRes[] data)
        {
            try
            {
                if (_defaultCreateObjFunc == null)
                {
                    return new OperationResultDto<TRes[]>(false, (int) HttpStatusCode.NotImplemented, 
                        ResponseMessageBuilder.GetNotImplementedErrorMessage(typeof(TObj).ToString(), "BulkCreate"), null);
                }
                IList<TObj> entities = data.Select(item => _defaultCreateObjFunc(item)).ToList();
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                await dbSet.AddRangeAsync(entities);
                
                int saveResult = await _dbContext.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    return new OperationResultDto<TRes[]>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetUnknownErrorMessage("BulkCreate", typeof(TObj).ToString()), null);
                }

                return new OperationResultDto<TRes[]>(true, (int) HttpStatusCode.Created, String.Empty,
                    entities.Select(item => _defaultCreateResFunc(item)).ToArray());
            }
            catch (Exception e)
            {
                string msg = ResponseMessageBuilder.GetCreateFailureMessage(typeof(TObj).ToString(), e.Message);
                _logger.LogError(msg);
                return new OperationResultDto<TRes[]>(false, (int) HttpStatusCode.InternalServerError, msg, null);
            }
        }

        /// <summary>
        ///     Method for update existing objects using EF, still have not default impl, but will be
        /// </summary>
        /// <param name="id">item identifier</param>
        /// <param name="data">>DTO with Model representation</param>
        /// <returns>DTO of updated object</returns>
        public virtual async Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data)
        {
            try
            {
                if (_defaultUpdateObjFunc == null)
                {
                    return new OperationResultDto<TRes>(false, (int) HttpStatusCode.NotImplemented, 
                        ResponseMessageBuilder.GetNotImplementedErrorMessage(typeof(TObj).ToString(), "Update"), null);
                }
                
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                TObj dbEntity = await dbSet.FirstOrDefaultAsync(item => item.Id.Equals(id));

                if (dbEntity == null)
                {
                    return new OperationResultDto<TRes>(false, (int) HttpStatusCode.NotFound,
                        ResponseMessageBuilder.GetUpdateNotFoundMessage(typeof(TObj).ToString(), id.ToString()), null);
                }

                _defaultUpdateObjFunc(data, id, dbEntity);

                int saveResult = await _dbContext.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    return new OperationResultDto<TRes>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetUnknownErrorMessage("Update", typeof(TObj).ToString()), null);
                }

                return new OperationResultDto<TRes>(true, (int) HttpStatusCode.OK, String.Empty,
                    _defaultCreateResFunc(dbEntity));
            }
            catch (Exception e)
            {
                string msg = ResponseMessageBuilder.GetUpdateFailureMessage(typeof(TObj).ToString(), id.ToString(), e.Message);
                _logger.LogError(msg);
                return new OperationResultDto<TRes>(false, (int) HttpStatusCode.InternalServerError, msg, null);
            }
        }

        /// <summary>
        /// Method for update existing objects in a database using Ef, in this class still have not a default impl, but will be
        /// </summary>
        /// <param name="data">Array of DTO with Model representation</param>
        /// <returns>Array of DTO of a updated objects</returns>
        public virtual async Task<OperationResultDto<TRes[]>> BulkUpdateAsync(TRes[] data)
        {
            try
            {
                if (_defaultUpdateObjFunc == null)
                {
                    return new OperationResultDto<TRes[]>(false, (int) HttpStatusCode.NotImplemented, 
                        ResponseMessageBuilder.GetNotImplementedErrorMessage(typeof(TObj).ToString(), 
                            "BulkUpdate"), null);
                }
                
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                IList<Tuple<TRes, TObj>> updatingObjects = new List<Tuple<TRes, TObj>>();
                // 1. construct Entity object from Resource due to Entity have a restriction - identifier
                foreach (TRes itemData in data)
                {
                    Tuple<TRes, TObj> item = new Tuple<TRes, TObj>(itemData, _defaultCreateObjFunc(itemData));
                    // 2. add only those objects that we build as Entities
                    if (item.Item2 != null)
                    {
                        updatingObjects.Add(item);
                    }
                }

                IList<TId> identifiers = updatingObjects.Select(item =>item.Item2.Id).ToList();
                IList<TObj> dbObjects = await dbSet.Where(item => identifiers.Contains(item.Id)).ToListAsync();

                // Important issue that we could not have an object in db but seems that we should update objects that were 
                // discovered
                foreach (TObj dbObject in dbObjects)
                {
                    Tuple<TRes, TObj> actualData = updatingObjects.First(o => o.Item2.Id.Equals(dbObject.Id));
                    _defaultUpdateObjFunc(actualData.Item1, dbObject.Id, dbObject);
                }
                
                int saveResult = await _dbContext.SaveChangesAsync();
                if (saveResult <= 0)
                {
                    return new OperationResultDto<TRes[]>(false, (int) HttpStatusCode.InternalServerError,
                        ResponseMessageBuilder.GetUnknownErrorMessage("BulkUpdate", typeof(TObj).ToString()), null);
                }

                return new OperationResultDto<TRes[]>(true, (int) HttpStatusCode.OK, string.Empty,
                    dbObjects.Select(i => _defaultCreateResFunc(i)).ToArray());

            }
            catch (Exception e)
            {
                string msg = ResponseMessageBuilder.GetBulkUpdateFailureMessage(typeof(TObj).ToString(), e.Message);
                _logger.LogError(msg);
                return new OperationResultDto<TRes[]>(false, (int) HttpStatusCode.InternalServerError, msg, null);
            }
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

        private readonly ILogger<EfModelManager<TRes, TObj, TId>> _logger;
        private readonly DbContext _dbContext;
        private readonly Func<TObj, TRes> _defaultCreateResFunc;
        private readonly Func<TRes, TObj> _defaultCreateObjFunc;
        private readonly Action<TRes, TId, TObj> _defaultUpdateObjFunc;
        private readonly Func<TObj, IDictionary<string, string>, bool> _filterFunc;
    }
}
