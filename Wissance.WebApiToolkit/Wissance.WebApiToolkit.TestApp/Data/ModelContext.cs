using Microsoft.EntityFrameworkCore;
using Wissance.WebApiToolkit.TestApp.Data.Entity;
using Wissance.WebApiToolkit.TestApp.Data.Entity.Mapping;

namespace Wissance.WebApiToolkit.TestApp.Data
{
    public class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            :base(options)
        {
        }
        
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<CodeEntity>().Map();
            modelBuilder.Entity<OrganizationEntity>().Map();
            modelBuilder.Entity<UserEntity>().Map();
            modelBuilder.Entity<RoleEntity>().Map();
        }

        public DbSet<CodeEntity> Codes { get; set; }
        public DbSet<OrganizationEntity> Organizations { get; set; }
        public DbSet<UserEntity> Users { get; set; }
    }
}