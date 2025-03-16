using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Tests.TestServer.Data.Entity
{
    public class RoleEntity : IModelIdentifiable<int>
    {
        public int Id { get; }
    }
}