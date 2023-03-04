using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductRefundItemConfiguration : IEntityTypeConfiguration<ProductRefundItemEF>
    {
        public void Configure(EntityTypeBuilder<ProductRefundItemEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Quantity).HasDatabaseName("Приход_Количество");
            builder.HasIndex(e => e.ProductRefundId).HasDatabaseName("IX_Приход_PN");
            builder.HasIndex(e => e.ProductId).HasDatabaseName("IX_Приход_tovID");
            
            builder.Property(e => e.Id).HasColumnName("КодОперации");
            builder.Property(e => e.Quantity).HasColumnName("Количество");
            builder.Property(e => e.ProductRefundId).HasColumnName("НомерПН");
            builder.Property(e => e.ProductId).HasColumnName("КодТовара");

            builder.Property(e => e.Price).HasColumnName("Цена").HasColumnType("money");
        }
    }
}
