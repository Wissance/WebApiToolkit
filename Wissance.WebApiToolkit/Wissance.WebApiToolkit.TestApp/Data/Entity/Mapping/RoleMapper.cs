using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity.Mapping
{
    internal static class RoleMapper
    {
        public static void Map(this EntityTypeBuilder<RoleEntity> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}