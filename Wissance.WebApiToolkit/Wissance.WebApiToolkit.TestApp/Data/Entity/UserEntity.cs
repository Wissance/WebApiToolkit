using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity
{
    public class UserEntity : IModelIdentifiable<int>
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public int OrganizationId { get; set; }
        public virtual OrganizationEntity Organization { get; set; }
    }
}