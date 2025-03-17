using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Entity
{
    public class UserEntity : IModelIdentifiable<int>
    {
        public int Id { get; }
    }
}