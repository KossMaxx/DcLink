using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class CashboxConfiguration : IEntityTypeConfiguration<CashboxEF>
    {
        public void Configure(EntityTypeBuilder<CashboxEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Kassa_ID");
            builder.Property(e => e.Description).HasColumnName("descr").HasMaxLength(30);
        }
    }
}
