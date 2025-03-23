using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity
{
    public class UserEntity : IModelIdentifiable<int>
    {
        public int Id { get; set; }
    }
}