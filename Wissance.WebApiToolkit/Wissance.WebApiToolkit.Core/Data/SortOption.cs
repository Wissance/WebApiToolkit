using System.Collections.Generic;

namespace Wissance.WebApiToolkit.Core.Data
{
    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public class SortOption
    {
        public SortOption(string sort, string sortOrder)
        {
            Sort = sort;
            if (string.IsNullOrEmpty(sortOrder))
            {
                Order = SortOrder.Ascending;
            }
            else
            {
                string lowerOrderVal = sortOrder.ToLower();
                if (_availableSortOptions.ContainsKey(lowerOrderVal))
                {
                    Order = _availableSortOptions[lowerOrderVal];
                }
                else
                {
                    Order = SortOrder.Ascending;
                }
            }
        }

        public string Sort { get; }
        public SortOrder Order { get; }

        private readonly IDictionary<string, SortOrder> _availableSortOptions = new Dictionary<string, SortOrder>()
        {
            {"asc", SortOrder.Ascending},
            {"desc", SortOrder.Descending}
        };
    }
}