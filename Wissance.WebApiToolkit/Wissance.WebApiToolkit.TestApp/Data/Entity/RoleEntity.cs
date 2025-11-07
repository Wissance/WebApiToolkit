using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity
{
    public class RoleEntity : IModelIdentifiable<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual IList<UserEntity> Users { get; set; }
    }
}