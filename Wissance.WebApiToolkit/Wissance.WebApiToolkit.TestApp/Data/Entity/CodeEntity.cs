
using System.Collections.Generic;
using Wissance.WebApiToolkit.Data.Entity;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity
{
    public class CodeEntity : IModelIdentifiable<int>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public virtual IList<OrganizationEntity> Organizations { get; set; }
    }
}