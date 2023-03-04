using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class SupplierHistoryPriceConfiguration : IEntityTypeConfiguration<SupplierHistoryPriceEF>
    {
        public void Configure(EntityTypeBuilder<SupplierHistoryPriceEF> builder)
        {
            builder.HasKey(e => e.Id)
                .HasName("PK_PriceHistory");
            builder.HasIndex(e => e.SupplierId)
                .HasDatabaseName("IX_klientID");
            builder.HasIndex(e => e.ProductId)
                .HasDatabaseName("IX_tovID");
            builder.HasIndex(e => e.Date)
                .HasDatabaseName("IX_dd");

            builder.Property(e => e.Id).HasColumnName("ii");
            builder.Property(e => e.Date)
                .HasColumnName("dd")
                .HasColumnType("datetime");
            builder.Property(e => e.SupplierId).HasColumnName("klientID");
            builder.Property(e => e.ProductId).HasColumnName("tovID");
            builder.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("money");

            #region AutoGeneretedConfiguration

            //entity.ToTable("priceHistory");

            //entity.Property(e => e.Nal)
            //    .HasColumnName("nal")
            //    .HasMaxLength(50);

            #endregion
        }
    }
}
