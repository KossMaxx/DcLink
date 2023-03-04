using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductCategoryParameterConfiguration : IEntityTypeConfiguration<ProductCategoryParameterEF>
    {
        public void Configure(EntityTypeBuilder<ProductCategoryParameterEF> builder)
        {
            builder.HasIndex(e => e.ProductId)
                .HasDatabaseName("kodtovara");

            builder.HasIndex(e => new { e.ProductId, e.ParameterId })
                .HasDatabaseName("IX_KategoryTovars_param");

            builder.HasIndex(e => new { e.Id, e.ProductId, e.ParameterId, e.CategoryId })
                .HasDatabaseName("IX_KategoryTovars_katID");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.CategoryId).HasColumnName("katID");

            builder.Property(e => e.ProductId).HasColumnName("kodtovara");

            builder.Property(e => e.ParameterId).HasColumnName("param");
        }
    }
}
