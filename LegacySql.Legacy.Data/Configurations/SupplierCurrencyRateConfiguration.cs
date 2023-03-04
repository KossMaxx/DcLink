using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class SupplierCurrencyRateConfiguration : IEntityTypeConfiguration<SupplierCurrencyRateEF>
    {
        public void Configure(EntityTypeBuilder<SupplierCurrencyRateEF> builder)
        {
            builder.ToTable("kurses");

            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.ClientId).HasColumnName("klientID");

            builder.Property(e => e.RateBn)
                .HasColumnName("kursBN")
                .HasColumnType("smallmoney");

            builder.Property(e => e.RateNal)
                .HasColumnName("kursNal")
                .HasColumnType("smallmoney");

            builder.Property(e => e.Date)
                .HasColumnName("ДатаИзм")
                .HasColumnType("datetime");

            builder.Property(e => e.RateDdr)
                .HasColumnName("kursDDP")
                .HasColumnType("smallmoney");

            builder.Property(e => e.Partner)
                .IsRequired()
                .HasColumnName("partner")
                .HasMaxLength(50);
        }
    }
}
