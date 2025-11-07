using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wissance.WebApiToolkit.Ef.Factories;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity.Mapping
{
    internal static class UserMapper
    {
        public static void Map(this EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasMany(p => p.Roles).WithMany(p => p.Users);
        }
    }
}