using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductDescriptionConfiguration : IEntityTypeConfiguration<ProductDescriptionEF>
    {
        public void Configure(EntityTypeBuilder<ProductDescriptionEF> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.ProductId, e.LanguageId })
                .HasDatabaseName("TBL_description_language");

            builder.HasIndex(e => new { e.Description, e.ProductId, e.LanguageId })
                .HasDatabaseName("tovId_language__description");

            builder.HasIndex(e => new { e.LanguageId, e.Uuu, e.Date })
                .HasDatabaseName("TBL_description_ddd");

            builder.HasIndex(e => new { e.ProductId, e.Description, e.LanguageId })
                .HasDatabaseName("ON_language_INCLUDE_tovID_description");

            builder.Property(e => e.Id).HasColumnName("description_ID");

            builder.Property(e => e.Date)
                .HasColumnName("ddd")
                .HasColumnType("datetime");

            builder.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.LanguageId).HasColumnName("language");

            builder.Property(e => e.ProductId).HasColumnName("tovID");

            builder.Property(e => e.Uuu)
                .HasColumnName("uuu")
                .HasMaxLength(10);
        }
    }
}
