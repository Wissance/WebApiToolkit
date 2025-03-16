using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Tests.TestServer.Data.Entity
{
    public class CodeEntity : IModelIdentifiable<int>
    {
        public int Id { get; }
    }
}