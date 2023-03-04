using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductSubtypeConfiguration : IEntityTypeConfiguration<ProductSubtypeEF>
    {
        public void Configure(EntityTypeBuilder<ProductSubtypeEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Title)
                .HasColumnName("nazv")
                .HasMaxLength(40);

            builder.Property(e => e.ProductTypeId).HasColumnName("tip");
            builder.Property(e => e.ChangedAt).HasColumnName("modified_at");
        }
    }
}
