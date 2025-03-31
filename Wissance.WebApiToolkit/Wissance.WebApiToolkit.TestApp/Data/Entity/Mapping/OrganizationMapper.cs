using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity.Mapping
{
    internal static class OrganizationMapper
    {
        public static void Map(this EntityTypeBuilder<OrganizationEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.ShortName).IsRequired();
            builder.Property(p => p.TaxNumber).IsRequired();
            builder.HasMany(p => p.Users)
                .WithOne(p => p.Organization)
                .HasForeignKey(p => p.OrganizationId);
            builder.HasMany(p => p.Codes).WithMany(p => p.Organizations);
        }
    }
}