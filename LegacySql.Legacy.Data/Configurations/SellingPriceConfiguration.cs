using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class SellingPriceConfiguration : IEntityTypeConfiguration<SellingPriceEF>
    {
        public void Configure(EntityTypeBuilder<SellingPriceEF> builder)
        {
            builder.HasIndex(e => e.ProductId)
                .HasDatabaseName("PK_tovar_log_tovID");

            builder.HasIndex(e => new { e.ProductId, e.ColumnId, e.Date })
                .HasDatabaseName("ТоварыLogSS_kolonka_zt");

            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.ProductId).HasColumnName("Kod_tov");

            builder.Property(e => e.ColumnId).HasColumnName("kolonka");

            builder.Property(e => e.Algorithm)
                .HasColumnName("prim")
                .HasMaxLength(50);

            builder.Property(e => e.Price)
                .HasColumnName("ss")
                .HasColumnType("money");

            builder.Property(e => e.Date)
                .HasColumnName("zt")
                .HasColumnType("datetime");

            builder.Ignore(e => e.Currency);
            builder.Ignore(e => e.ProductDateLastPriceChange);
            builder.Ignore(e => e.IsCash);
        }
    }
}
