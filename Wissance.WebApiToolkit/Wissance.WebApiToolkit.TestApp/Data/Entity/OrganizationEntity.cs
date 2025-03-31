using System.Collections.Generic;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity
{
    public class OrganizationEntity : IModelIdentifiable<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string TaxNumber { get; set; }
        public virtual IList<UserEntity> Users { get; set; }
        public virtual IList<CodeEntity> Codes { get; set; }
    }
}