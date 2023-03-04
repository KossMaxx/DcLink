using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class IncomingBillConfiguration : IEntityTypeConfiguration<IncomingBillEF>
    {
        public void Configure(EntityTypeBuilder<IncomingBillEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("Код_счета");
            builder.Property(e => e.IncomingNumber).HasColumnName("Номер");
            builder.Property(e => e.Date).HasColumnName("дата");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.RecipientId).HasColumnName("OOO");
            builder.Property(e => e.ChangedAt).HasColumnName("DataLastChange");
            builder.Property(e => e.SupplierId).HasColumnName("FirmID");
        }
    }
}
