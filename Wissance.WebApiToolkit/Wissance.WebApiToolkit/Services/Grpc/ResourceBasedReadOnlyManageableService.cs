using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Wissance.WebApiToolkit.Data;
using Wissance.WebApiToolkit.Dto;
using Wissance.WebApiToolkit.Managers;

namespace Wissance.WebApiToolkit.Services.Grpc
{
    public abstract class ResourceBasedReadOnlyManageableService<TRes, TData, TId> : IResourceBasedReadOnlyService<TRes, TData, TId>
    where TRes : class
    {
        public virtual async Task<PagedDataDto<TRes>> ReadAsync(int? page, int? size, string sort, string order, IDictionary<string, string> filterParams)
        {
            int pageNumber = GetPage(page);
            int pageSize = GetPageSize(size);
            SortOption sorting = !string.IsNullOrEmpty(sort) ? new SortOption(sort, order) : null;
            OperationResultDto<Tuple<IList<TRes>, long>> result = await Manager.GetAsync(pageNumber, pageSize, sorting, filterParams);
            return new PagedDataDto<TRes>(pageNumber, result.Data.Item2, GetTotalPages(result.Data.Item2, pageSize), result.Data.Item1);
        }

        public virtual async Task<TRes> ReadByIdAsync(TId id)
        {
            throw new System.NotImplementedException();
        }
        
        private long GetTotalPages(long totalItems, int pageSize)
        {
            if (pageSize <= 0)
            {
                // todo(UMV): this is hardly ever possible but add logging here for jokers
                return -1;
            }

            return (long)Math.Ceiling((double)totalItems / pageSize);
        }
        
        protected int GetPage(int? page)
        {
            int selectedPage = page ?? DefaultPage;
            return selectedPage < 1 ? 1 : selectedPage;
        }

        protected int GetPageSize(int? size)
        {
            return size ?? DefaultSize;
        }
        
        public IModelManager<TRes, TData, TId> Manager { get; set; }
        
        private const int DefaultPage = 1;
        private const int DefaultSize = 25;
    }
}