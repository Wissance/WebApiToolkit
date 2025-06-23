using System.Collections.Generic;

namespace Wissance.WebApiToolkit.Core.Data
{
    public interface IReadFilterable
    {
        IDictionary<string, string> SelectFilters();
    }
}