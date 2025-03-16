using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.Tests.TestServer.Data.Entity
{
    public class UserEntity : IModelIdentifiable<int>
    {
        public int Id { get; }
    }
}