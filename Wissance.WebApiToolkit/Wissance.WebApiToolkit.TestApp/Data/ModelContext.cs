using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Entity;

namespace Wissance.WebApiToolkit.TestApp.Data
{
    public class ModelContext : DbContext
    { 
        public DbSet<CodeEntity> Codes { get; set; }
    }
}