using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Entity
{
    public class RoleEntity : IModelIdentifiable<int>
    {
        public int Id { get; }
    }
}