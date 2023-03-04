using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class NotInStockStatusConfiguration : IEntityTypeConfiguration<NotInStockStatusEF>
    {
        public void Configure(EntityTypeBuilder<NotInStockStatusEF> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("ii");

            builder.Property(e => e.Name)
                .HasColumnName("statusname")
                .HasMaxLength(50);
        }
    }
}
