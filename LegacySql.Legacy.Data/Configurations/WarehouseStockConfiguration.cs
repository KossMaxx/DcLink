using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class WarehouseStockConfiguration : IEntityTypeConfiguration<WarehouseStockEF>
    {
        public void Configure(EntityTypeBuilder<WarehouseStockEF> builder)
        {
            builder.HasIndex(e => new { e.ProductId, e.WarehouseId, Stock = e.Quantity })
                .HasDatabaseName("Sklad_otdel_nal");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.ProductId).HasColumnName("tovID");

            builder.Property(e => e.Quantity).HasColumnName("Qfree");

            builder.Property(e => e.WarehouseId).HasColumnName("skladID");
        }
    }
}
