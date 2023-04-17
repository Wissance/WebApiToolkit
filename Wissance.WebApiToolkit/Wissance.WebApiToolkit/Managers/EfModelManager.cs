using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.Data.EfContext;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Managers
{
    /// <summary>
    ///    This is a Model Manager for working with EntityFramework ORM as a tool for perform CRUD operations over persistent objects
    ///    It has a default implementation of the following method of IModelManager:
    ///    * GetAsync method for obtain many items
    ///    * GetByIdAsync method for obtain one item by id
    ///    * Delete method 
    /// </summary>
    /// <typeparam name="TObj">Model class deriving from IModelIdentifiable</typeparam>
    /// <typeparam name="TRes">DTO class</typeparam>
    /// <typeparam name="TId">Identifier type that is using as database PK</typeparam>
    public abstract class EfModelManager <TObj, TRes, TId> : IModelManager<TRes, TObj, TId>
                                                where TObj: class, IModelIdentifiable<TId>
                                                where TRes: class
                                                where TId: IComparable
             
    {
        /// <summary>
        ///    Constructor of this abstract class
        /// </summary>
        /// <param name="dbContext">Context derived from EfDbContext </param>
        /// <param name="createFunc">Delegate for creating DTO from Model</param>
        /// <param name="loggerFactory">Logger factory</param>
        /// <exception cref="ArgumentNullException"></exception>
        public EfModelManager(EfDbContext dbContext, Func<TObj, TRes> createFunc, ILoggerFactory loggerFactory)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext");
            _logger = loggerFactory.CreateLogger<EfModelManager<TObj, TRes, TId>>();
            _defaultCreateFunc = createFunc;
        }

        public async Task<OperationResultDto<Tuple<IList<TRes>, long>>> GetManyAsync<TKey>(int page, int size, 
                                                      Func<TObj, bool> filter, Func<TObj, TKey> sort,
                                                      Func<TObj, TRes> createFunc = null)
        {
            try
            {
                //IQueryable<TObj> filteredObjects = dbSet;
                long totalItems = 0;
                DbSet<TObj> dbSet = _dbContext.Get<TObj, TId>();
                IList<TObj> entities = null;
                if (sort != null)
                {
                    entities = dbSet.OrderBy(sort).ToList();
                }

                if (filter != null)
                {
                    // filteredObjects = await dbSet.ToListAsync();
                    if (entities == null)
                    {
                        entities = await dbSet.ToListAsync();
                    }

                    entities = entities.Where(o => filter(o)).Skip(size * (page - 1)).Take(size).ToList();
                }
                else
                {
                    if (entities == null)
                        entities = await dbSet.Skip(size * (page - 1)).Take(size).ToListAsync();
                    else
                    {
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

        public async Task<OperationResultDto<TRes>> GetOneAsync(TId id, Func<TObj, TRes> createFunc = null)
        {
            try
            {
                DbSet<TObj> dbSet = _dbContext.Get<TObj, TId>();
                TObj entity = await dbSet.FirstOrDefaultAsync(i => i.Id.Equals(id));
                if (entity == null)
                    return new OperationResultDto<TRes>(false, (int)HttpStatusCode.NotFound, 
                                                        GetResourceNotFoundMessage(typeof(TObj).ToString(), id), null);
                return new OperationResultDto<TRes>(true, (int)HttpStatusCode.OK, null, 
                    createFunc != null?createFunc(entity): _defaultCreateFunc(entity));
            }
            catch (Exception e)
            {
                _logger.LogError($"An error: {e.Message} occurred during object of type: {typeof(TObj)} with id: {id} retrieve and convert to object of type: {typeof(TRes)}");
                return new OperationResultDto<TRes>(false, (int)HttpStatusCode.NotFound,
                                                    GetResourceNotFoundMessage(typeof(TObj).ToString(), id), null);
            }
        }

        public async Task<OperationResultDto<Tuple<IList<TRes>, long>>> GetAsync(int page, int size)
        {
            // this method is using default sorting and order, if specific order or sorting is required please specify it using another GetAsync method
            return await GetManyAsync<TRes>(page, size, null, null);
        }
        
        public async Task<OperationResultDto<TRes>> GetByIdAsync(TId id)
        {
            return await GetOneAsync(id);
        }
        
        public virtual Task<OperationResultDto<TRes>> CreateAsync(TRes data)
        {
            throw new NotImplementedException();
        }

        public virtual Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data)
        {
            throw new NotImplementedException();
        }
        
        public async Task<OperationResultDto<bool>> DeleteAsync(TId id)
        {
            try
            {
                DbSet<TObj> dbSet = _dbContext.Get<TObj, TId>();
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


        public string GetCreateFailureMessage(string entity, string exceptionMessage)
        {
            return string.Format(CreateFailureMessageTemplate, entity, exceptionMessage);
        }

        public string GetResourceNotFoundMessage(string resource, TId id)
        {
            return string.Format(ResourceNotFoundTemplate, resource, id);
        }

        public string GetUpdateFailureMessage(string entity, int id, string exceptionMessage)
        {
            return string.Format(UpdateFailureMessageTemplate, entity, id, exceptionMessage);
        }

        public string GetUpdateNotFoundMessage(string entity, int id)
        {
            return string.Format(UpdateFailureNotFoundMessageTemplate, entity, id);
        }

        public string GetCurrentUserResourceAccessErrorMessage(string resource)
        {
            return string.Format(CurrentUserIsNotResourceOwnerTemplate, resource);
        }

        public string GetUnknownErrorMessage(string operation, string resource)
        {
            return string.Format(UnknownErrorMessageTemplate, operation, resource);
        }


        private const string ResourceNotFoundTemplate = "Recource of type \"{0}\" with id: {1} was not found";
        private const string CurrentUserIsNotResourceOwnerTemplate = "Current user is not \"{0}\" owner";
        private const string CreateFailureMessageTemplate = "An error occurred during \"{0}\" create with error: {1}";
        private const string UpdateFailureMessageTemplate = "An error occurred during \"{0}\" update with id: \"{1}\", error: {2}";
        private const string UpdateFailureNotFoundMessageTemplate = "{0} with id: {1} was not found";

        private const string UnknownErrorMessageTemplate = "An error occurred during {0} \"{1}\", contact system maintainer";
        public const string UserNotAuthenticatedMessage = "User is not authenticated";

        private readonly ILogger<EfModelManager<TObj, TRes, TId>> _logger;
        private readonly EfDbContext _dbContext;
        private readonly Func<TObj, TRes> _defaultCreateFunc;
    }
}
