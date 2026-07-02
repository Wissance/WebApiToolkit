using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity.Mapping
{
    internal static class ProfileMapper
    {
        public static void Map(this EntityTypeBuilder<ProfileEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.User).WithOne(p => p.Profile)
                .HasForeignKey<ProfileEntity>(p => p.UserId);
        }
    }
}