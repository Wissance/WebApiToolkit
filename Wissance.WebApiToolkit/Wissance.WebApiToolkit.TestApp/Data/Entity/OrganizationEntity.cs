using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Entity
{
    public class OrganizationEntity : IModelIdentifiable<int>
    {
        public int Id { get; }
    }
}