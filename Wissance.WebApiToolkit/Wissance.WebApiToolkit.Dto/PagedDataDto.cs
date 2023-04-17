using System;
using System.Collections.Generic;

namespace Wissance.WebApiToolkit.Dto
{
    /// <summary>
    ///    DTO that represents a collection of items of a same type i.e. a result of querying GET /api/controller/?page={p}&size={s}
    /// </summary>
    /// <typeparam name="T">Type representing REST resource</typeparam>
    public class PagedDataDto <T> where T: class
    {
        public PagedDataDto()
        {
            Data = new List<T>();
        }

        public PagedDataDto(long page, long total, long pages, IList<T> data)
        {
            Page = page;
            Total = total;
            Pages = pages;
            Data = data;
        }

        /// <summary>
        ///    Page is a number of data portion with specific size from beginning
        /// </summary>
        public long Page { get; set; }
        /// <summary>
        ///    Total is a total number of items 
        /// </summary>
        public long Total { get; set; }
        /// <summary>
        ///    Pages is a total number of pages of a specified size
        /// </summary>
        public long Pages { get; set; }
        /// <summary>
        ///    Data portion itself
        /// </summary>
        public IList<T> Data { get; set; }
    }
}
