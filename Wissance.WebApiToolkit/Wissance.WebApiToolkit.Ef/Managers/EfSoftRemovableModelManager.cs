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
    public abstract class EfSoftRemovableModelManager<TRes, TObj, TId> : EfModelManager<TRes, TObj, TId>
        where TObj: class, IModelIdentifiable<TId>, IModelSoftRemovable
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
        public EfSoftRemovableModelManager(DbContext dbContext, Func<TObj, IDictionary<string, string>, bool> filterFunc, 
                                           Func<TObj, TRes> createResFunc, Func<TRes, TObj> createObjFunc,
                                           Action<TRes, TId, TObj> updateObjFunc,
                                           ILoggerFactory loggerFactory)
            :base(dbContext, filterFunc, createResFunc, createObjFunc, updateObjFunc, loggerFactory)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
            _logger = loggerFactory.CreateLogger<EfSoftRemovableModelManager<TRes, TObj, TId>>();
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
        public override async Task<OperationResultDto<Tuple<IList<TRes>, long>>> GetManyAsync<TF>(int page, int size, IDictionary<string, string> parameters, SortOption sorting,
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
                            entities = Queryable.Where<TObj>(dbSet, m => !m.IsDeleted).OrderBy(sortFunc).ToList();
                        }
                        else
                        {
                            entities = Queryable.Where<TObj>(dbSet, m => !m.IsDeleted).OrderByDescending(sortFunc).ToList();
                        }
                    }
                }

                if (filterFunc != null)
                {
                    // filteredObjects = await dbSet.ToListAsync();
                    if (entities == null)
                    {
                        entities = await Queryable.Where<TObj>(dbSet, m => !m.IsDeleted).ToListAsync();
                    }

                    IEnumerable<TObj> items = entities.Where(o => filterFunc(o, parameters));
                    totalItems = items.Count();
                    entities = items.Skip(size * (page - 1)).Take(size).ToList();
                }
                else
                {
                    totalItems = await Queryable.Where<TObj>(dbSet, m => !m.IsDeleted).LongCountAsync();
                    if (entities == null)
                    {
                        entities = await Queryable.Where<TObj>(dbSet, m => !m.IsDeleted).Skip(size * (page - 1)).Take(size).ToListAsync();
                    }
                    else
                    {
                        entities = entities.Skip(size * (page - 1)).Take(size).ToList();
                    }
                }
                
                return new OperationResultDto<Tuple<IList<TRes>, long>>(true, (int)HttpStatusCode.OK, null,
                    new Tuple<IList<TRes>, long>(entities.Select(e => createFunc!=null ? createFunc(e) : _defaultCreateResFunc(e)).ToList(), totalItems));
            }
            catch (Exception e)
            {
                _logger.LogError($"An error: {e.Message} occurred during collection of object of type: {typeof(TObj)} retrieve and convert to objects of type: {typeof(TRes)}");
                return new OperationResultDto<Tuple<IList<TRes>, long>>(true, (int)HttpStatusCode.InternalServerError, 
                    ResponseMessageBuilder.GetOperationErrorMessage(typeof(TObj).ToString(), "GetMany", e.Message), 
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
        public override async Task<OperationResultDto<TRes>> GetOneAsync(TId id, Func<TObj, TRes> createFunc = null)
        {
            try
            {
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                TObj entity = await dbSet.FirstOrDefaultAsync(i => i.Id.Equals(id));
                if (entity == null || entity.IsDeleted)
                    return new OperationResultDto<TRes>(false, (int)HttpStatusCode.NotFound, 
                                                        ResponseMessageBuilder.GetResourceNotFoundMessage(typeof(TObj).ToString(), id), null);
                return new OperationResultDto<TRes>(true, (int)HttpStatusCode.OK, null, 
                    createFunc != null?createFunc(entity): _defaultCreateResFunc(entity));
            }
            catch (Exception e)
            {
                _logger.LogError($"An error: {e.Message} occurred during object of type: {typeof(TObj)} with id: {id} retrieve and convert to object of type: {typeof(TRes)}");
                return new OperationResultDto<TRes>(false, (int)HttpStatusCode.NotFound,
                                                    ResponseMessageBuilder.GetOperationErrorMessage(typeof(TObj).ToString(), "GetOne", e.Message), null);
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
        public override async Task<OperationResultDto<Tuple<IList<TRes>, long>>> GetAsync(int page, int size, SortOption sorting = null, 
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
        /// DeleteAsync method for remove object from Database using Ef
        /// </summary>
        /// <param name="id">item identifier</param>
        /// <returns>true if removal was successful, otherwise false</returns>
        public override async Task<OperationResultDto<bool>> DeleteAsync(TId id)
        {
            try
            {
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                TObj item = await dbSet.FirstOrDefaultAsync(t => t.Id.Equals(id));

                if (item == null || item.IsDeleted)
                    return new OperationResultDto<bool>(false, (int)HttpStatusCode.NotFound, "Item does not exists", false);
                item.IsDeleted = true;
                await _dbContext.SaveChangesAsync();
                return new OperationResultDto<bool>(true, (int)HttpStatusCode.NoContent, null, true);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during object of type: {nameof(TObj)} with id: {id} remove: {e.Message}");
                return new OperationResultDto<bool>(false, (int)HttpStatusCode.InternalServerError, 
                    ResponseMessageBuilder.GetOperationErrorMessage(typeof(TObj).ToString(), "Delete", e.Message), false);
            }
        }
        
        /// <summary>
        /// BulkDeleteAsync method for remove object from Database using Ef
        /// </summary>
        /// <param name="objectsIds">item identifiers</param>
        /// <returns>true if removal was successful, otherwise false</returns>
        public override async Task<OperationResultDto<bool>> BulkDeleteAsync(TId[] objectsIds)
        {
            try
            {
                DbSet<TObj> dbSet = _dbContext.Set<TObj>();
                IList<TObj> items = await dbSet.Where(t => objectsIds.Contains(t.Id)).ToListAsync();

                if (items == null || !items.Any())
                    return new OperationResultDto<bool>(false, (int)HttpStatusCode.NotFound, "Items are not exists", false);
                foreach (TObj item in items)
                {
                    item.IsDeleted = true;
                }
                // todo(UMV): test and add result check as in Create && Update
                await _dbContext.SaveChangesAsync();
                return new OperationResultDto<bool>(true, (int)HttpStatusCode.NoContent, null, true);
            }
            catch (Exception e)
            {
                string msg = ResponseMessageBuilder.GetBulkDeleteFailureMessage(typeof(TObj).ToString(), e.Message);
                return new OperationResultDto<bool>(false, (int) HttpStatusCode.InternalServerError, msg, false);
            }
        }

        private readonly ILogger<EfSoftRemovableModelManager<TRes, TObj, TId>> _logger;
        private readonly DbContext _dbContext;
        private readonly Func<TObj, TRes> _defaultCreateResFunc;
        private readonly Func<TRes, TObj> _defaultCreateObjFunc;
        private readonly Action<TRes, TId, TObj> _defaultUpdateObjFunc;
        private readonly Func<TObj, IDictionary<string, string>, bool> _filterFunc;
    }
}