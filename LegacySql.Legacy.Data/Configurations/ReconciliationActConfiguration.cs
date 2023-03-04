using LegacySql.Legacy.Data.Models;using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ReconciliationActConfiguration : IEntityTypeConfiguration<ReconciliationActEF>
    {
        public void Configure(EntityTypeBuilder<ReconciliationActEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Sum).HasColumnName("s1").HasColumnType("money");
            builder.Property(e => e.ChangedAt).HasColumnName("d1");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.IsApproved).HasColumnName("f");
        }
    }
}
