using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class SupplierActualPriceConfiguration : IEntityTypeConfiguration<SupplierActualPriceEF>
    {
        public void Configure(EntityTypeBuilder<SupplierActualPriceEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => new { e.SupplierId, e.SupplierProductCode })
                .HasDatabaseName("NonClusteredIndex-20170712-pos-partner");

            builder.HasIndex(e => new { e.ProductId, e.SupplierProductCode, e.SupplierId })
                .HasDatabaseName("Prices_partner");

            builder.HasIndex(e => new { e.ProductId, e.Price, e.SupplierId })
                .HasDatabaseName("Цены партнер");

            builder.HasIndex(e => new { e.ProductId, e.SupplierId, e.IsInStock, e.Price, e.Date })
                .HasDatabaseName("IX_PriceAlgoritmKonkurentRozn");

            builder.HasIndex(e => new { e.ProductId, e.Price, e.PriceRetail, e.SupplierId, e.Date })
                .HasDatabaseName("NameofIndex");

            builder.HasIndex(e => new { e.Price, e.PriceRetail, e.ProductId, e.SupplierId, e.Date })
                .HasDatabaseName("price_priceRozn");

            builder.HasIndex(e => new { e.Id, e.ProductId, e.Price, e.PriceRetail, e.SupplierProductCode, e.IsInStock, e.Date, e.SupplierId })
                .HasDatabaseName("Ceny_partner");

            builder.Property(e => e.Id).HasColumnName("tovID");

            builder.Property(e => e.Date)
                .HasColumnName("dataP")
                .HasColumnType("datetime");

            builder.Property(e => e.ProductId).HasColumnName("KodTovara");

            builder.Property(e => e.Monitor).HasColumnName("monitor");

            builder.Property(e => e.SupplierId).HasColumnName("partner");

            builder.Property(e => e.SupplierProductCode)
                .HasColumnName("pos")
                .HasMaxLength(255);

            builder.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("money");

            builder.Property(e => e.PriceDialer)
                .HasColumnName("priceDiler")
                .HasColumnType("money");

            builder.Property(e => e.PriceRetail)
                .HasColumnName("priceRozn")
                .HasColumnType("money");

            builder.Property(e => e.IsInStock)
                .HasColumnName("nal")
                .HasMaxLength(50);
            builder.Ignore(e => e.Currency);

            #region AutoGeneretedConfiguration

            //entity.Property(e => e.Nalf).HasColumnName("nalf");

            //entity.Property(e => e.PriceOpt)
            //    .HasColumnName("priceOpt")
            //    .HasColumnType("money");

            //entity.Property(e => e.Rebate)
            //    .HasColumnName("rebate")
            //    .HasColumnType("money");

            //entity.Property(e => e.Reserved).HasColumnName("reserved");

            #endregion
        }
    }
}
