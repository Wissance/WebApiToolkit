using System.Collections.Generic;

namespace Wissance.WebApiToolkit.Dto
{
    public class PagedDataDto <T> where T: class
    {
        public PagedDataDto()
        {
            Data = new List<T>();
        }

        public PagedDataDto(int page, IList<T> data)
        {
            Page = page;
            Data = data;
        }

        public int Page { get; set; }
        public IList<T> Data { get; set; }
    }
}
