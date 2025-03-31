using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wissance.WebApiToolkit.TestApp.Data.Entity.Mapping
{
    internal static class CodeMapper
    {
        public static void Map(this EntityTypeBuilder<CodeEntity> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Code).IsRequired();
            builder.Property(p => p.Name).IsRequired();
        }
    }
}