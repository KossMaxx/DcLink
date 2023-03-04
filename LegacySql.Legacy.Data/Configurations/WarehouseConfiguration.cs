using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<WarehouseEF>
    {
        public void Configure(EntityTypeBuilder<WarehouseEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("sklad_ID");
            builder.Property(e => e.Description).HasColumnName("sklad_desc").HasMaxLength(30);
            builder.Property(e => e.IsСommission).HasColumnName("consig");
        }
    }
}
