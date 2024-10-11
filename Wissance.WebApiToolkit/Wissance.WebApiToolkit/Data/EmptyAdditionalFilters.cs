using System.Collections.Generic;

namespace Wissance.WebApiToolkit.Data
{
    public class EmptyAdditionalFilters : IReadFilterable
    {
        public IDictionary<string, string> SelectFilters()
        {
            return new Dictionary<string, string>();
        }
    }
}