using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity.Mapping
{
    internal static class UserMapper
    {
        public static void Map(this EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}