using System.Collections.Generic;

namespace Wissance.WebApiToolkit.Data
{
    public interface IReadFilterable
    {
        IDictionary<string, string> SelectFilters();
    }
}