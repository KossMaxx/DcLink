using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    internal class BillItemConfiguration : IEntityTypeConfiguration<BillItemEF>
    {
        public void Configure(EntityTypeBuilder<BillItemEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("ID");
            builder.Property(e => e.BillId).HasColumnName("Код_счета");
            builder.Property(e => e.NomenclatureId).HasColumnName("Кодтовара");
            builder.Property(e => e.Price).HasColumnName("ЦенаУ");
            builder.Property(e => e.PriceUAH).HasColumnName("ЦенаГрн");
            builder.Property(e => e.Quantity).HasColumnName("Кол");
            builder.Property(e => e.Amount).HasColumnName("СуммаФ");
            builder.Property(e => e.AmountUAH).HasColumnName("СуммаГРН");
        }
    }
}
