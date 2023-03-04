using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class CashboxApplicationPaymentConfiguration : IEntityTypeConfiguration<CashboxApplicationPaymentEF>
    {
        public void Configure(EntityTypeBuilder<CashboxApplicationPaymentEF> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("docID");
            builder.Property(e => e.Date).HasColumnName("createDate");
            builder.Property(e => e.WriteOffCliectId).HasColumnName("balFromID");
            builder.Property(e => e.ReceiveClientId).HasColumnName("balMoveToID");
            builder.Property(e => e.Amount).HasColumnName("amountCash");
            builder.Property(e => e.ChangeDate).HasColumnName("changeDate");
            builder.Property(e => e.CurrencyId).HasColumnName("amountCurType");
            builder.Property(e => e.Description).HasColumnName("note");
            builder.Property(e => e.HeldIn).HasColumnName("heldIn");
        }
    }
}
