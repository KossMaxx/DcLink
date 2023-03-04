using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductVideoConfiguration : IEntityTypeConfiguration<ProductVideoEF>
    {
        public void Configure(EntityTypeBuilder<ProductVideoEF> builder)
        {
            builder.ToTable("VideoURL");
            builder.Property(e => e.Id).HasColumnName("id");

            builder.HasIndex(e => new { e.Url, e.ProductId })
                .HasDatabaseName("NonClusteredIndex-20170606-122822");

            builder.Property(e => e.Date)
                .HasColumnName("ddd")
                .HasColumnType("datetime");

            builder.Property(e => e.ProductId).HasColumnName("item_id");

            builder.Property(e => e.Url)
                .IsRequired()
                .HasColumnName("url")
                .HasMaxLength(150);
        }
    }
}
