using System;

namespace Wissance.WebApiToolkit.Core.Utils
{
    public static class PagingUtils
    {
        public static long GetTotalPages(long totalItems, int pageSize)
        {
            if (pageSize <= 0)
            {
                // todo(UMV): this is hardly ever possible but add logging here for jokers
                return -1;
            }

            return (long)Math.Ceiling((double)totalItems / pageSize);
        }
        
        public static int GetPage(int? page)
        {
            int selectedPage = page ?? DefaultPage;
            return selectedPage < 1 ? 1 : selectedPage;
        }

        public static int GetPageSize(int? size)
        {
            int selectedSize = size ?? DefaultSize;
            return selectedSize <= 0 ? DefaultSize : selectedSize;
        }
        
        private const int DefaultPage = 1;
        private const int DefaultSize = 25;
    }
}