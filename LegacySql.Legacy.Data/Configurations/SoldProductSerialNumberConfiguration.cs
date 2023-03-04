using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class SoldProductSerialNumberConfiguration : IEntityTypeConfiguration<SoldProductSerialNumberEF>
    {
        public void Configure(EntityTypeBuilder<SoldProductSerialNumberEF> builder)
        {
            builder.HasIndex(e => e.ClientOrderItemId)
                .HasDatabaseName("rashod_Num");

            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.ClientOrderItemId).HasColumnName("Num");
            builder.Property(e => e.ClientOrderItemArchivalId).HasColumnName("Num");

            builder.Property(e => e.SerialNumber).HasColumnName("Snom").HasMaxLength(50);
        }
    }
}
