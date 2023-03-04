using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    internal class IncomingBillItemConfiguration : IEntityTypeConfiguration<IncomingBillItemEF>
    {
        public void Configure(EntityTypeBuilder<IncomingBillItemEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("ID");
            builder.Property(e => e.IncomingBillId).HasColumnName("Код_счета");
            builder.Property(e => e.NomenclatureId).HasColumnName("Кодтовара");
            builder.Property(e => e.PriceUAH).HasColumnName("ЦенаГрн");
            builder.Property(e => e.Quantity).HasColumnName("Кол");
        }
    }
}
