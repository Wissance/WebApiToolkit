using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.Data.Entity;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Managers
{
    public abstract class ModelManager <TObj, TRes, TId> : IModelManager<TRes, TObj, TId>
                                                where TObj: class, IModelIdentifiable<TId>
                                                where TRes: class
                                                where TId: IComparable
             
    {
        public ModelManager(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ModelManager<TObj, TRes, TId>>();
        }

        public async Task<OperationResultDto<IList<TRes>>> GetAsync<TKey>(DbSet<TObj> dbSet, int page, int size, 
                                                      Func<TObj, bool> filter, Func<TObj, TKey> sort,
                                                      Func<TObj, TRes> createFunc)
        {
            try
            {
                // IQueryable<TObj> filteredObjects = dbSet;
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

                return new OperationResultDto<IList<TRes>>(true, (int)HttpStatusCode.OK, null,
                    entities.Select(e => createFunc(e)).ToList());
            }
            catch (Exception e)
            {
                _logger.LogError($"An error: {e.Message} occurred during collection of object of type: {typeof(TObj)} retrieve and convert to objects of type: {typeof(TRes)}");
                return new OperationResultDto<IList<TRes>>(true, (int)HttpStatusCode.InternalServerError, "Error occurred, contact system maintainer", null);
            }
        }

        public async Task<OperationResultDto<TRes>> GetAsync(DbSet<TObj> dbSet, TId id, Func<TObj, TRes> createFunc)
        {
            try
            {
                TObj entity = await dbSet.FirstOrDefaultAsync(i => i.Id.Equals(id));
                if (entity == null)
                    return new OperationResultDto<TRes>(false, (int)HttpStatusCode.NotFound, 
                                                        GetResourceNotFoundMessage(typeof(TObj).ToString(), id), null);
                return new OperationResultDto<TRes>(true, (int)HttpStatusCode.OK, null, createFunc(entity));
            }
            catch (Exception e)
            {
                _logger.LogError($"An error: {e.Message} occurred during object of type: {typeof(TObj)} with id: {id} retrieve and convert to object of type: {typeof(TRes)}");
                return new OperationResultDto<TRes>(false, (int)HttpStatusCode.NotFound,
                                                    GetResourceNotFoundMessage(typeof(TObj).ToString(), id), null);
            }
        }

        public async Task<OperationResultDto<bool>> DeleteAsync(DbContext context, DbSet<TObj> dbSet, TId id)
        {
            try
            {
                TObj item = await dbSet.FirstOrDefaultAsync(t => t.Id.Equals(id));

                if (item == null)
                    return new OperationResultDto<bool>(false, (int)HttpStatusCode.NotFound, "Item does not exists", false);
                dbSet.Remove(item);
                await context.SaveChangesAsync();
                return new OperationResultDto<bool>(true, (int)HttpStatusCode.NoContent, null, true);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred during object of type: {nameof(TObj)} with id: {id} remove: {e.Message}");
                return new OperationResultDto<bool>(false, (int)HttpStatusCode.InternalServerError, "Error occurred during object delete, contact system maintainer", false);
            }
        }

        public virtual Task<OperationResultDto<TRes>> CreateAsync(TRes data)
        {
            throw new NotImplementedException();
        }

        public virtual Task<OperationResultDto<TRes>> UpdateAsync(TId id, TRes data)
        {
            throw new NotImplementedException();
        }

        public virtual Task<OperationResultDto<bool>> DeleteAsync(TId id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<OperationResultDto<IList<TRes>>> GetAsync(int page, int size)
        {
            throw new NotImplementedException();
        }

        public virtual Task<OperationResultDto<TRes>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
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

        private readonly ILogger<ModelManager<TObj, TRes, TId>> _logger;
    }
}
