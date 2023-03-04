using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LegacySql.Legacy.Data.Configurations
{
    public class CashboxPaymentConfiguration : IEntityTypeConfiguration<CashboxPaymentEF>
    {
        public void Configure(EntityTypeBuilder<CashboxPaymentEF> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("код");
            builder.Property(e => e.Date).HasColumnName("дата");
            builder.Property(e => e.CashboxId).HasColumnName("Kassa_ID");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.AmountUSD).HasColumnName("ден");
            builder.Property(e => e.AmountUAH).HasColumnName("грн");
            builder.Property(e => e.AmountEuro).HasColumnName("евро");
            builder.Property(e => e.Rate).HasColumnName("курс");
            builder.Property(e => e.RateEuro).HasColumnName("курсевро");
            builder.Property(e => e.Description).HasColumnName("Прим");
            builder.Property(e => e.CreateUsername).HasColumnName("sozdal");
            builder.Property(e => e.CreateDate).HasColumnName("dsozd");
            builder.Property(e => e.ChangeUsername).HasColumnName("lastuser");
            builder.Property(e => e.ChangeDate).HasColumnName("lasttime");
        }
    }
}
