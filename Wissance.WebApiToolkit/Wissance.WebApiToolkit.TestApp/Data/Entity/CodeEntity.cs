
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity
{
    public class CodeEntity : IModelIdentifiable<int>
    {
        public int Id { get; }
        public string Code { get; set; }
    }
}