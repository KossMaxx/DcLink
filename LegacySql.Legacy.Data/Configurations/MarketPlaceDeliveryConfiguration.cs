using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class MarketPlaceDeliveryConfiguration : IEntityTypeConfiguration<MarketplaceDeliveryEF>
    {
        public void Configure(EntityTypeBuilder<MarketplaceDeliveryEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.WarehouseNumber).HasColumnName("place_number").HasMaxLength(16).IsUnicode(false);
            builder.Property(e => e.WarehouseId).HasColumnName("ref_id").HasMaxLength(64).IsUnicode(false);
            builder.Property(e => e.MarketplaceNumber).HasColumnName("order_id");
        }
    }
}
