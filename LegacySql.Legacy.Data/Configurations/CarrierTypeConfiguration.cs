using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class CarrierTypeConfiguration : IEntityTypeConfiguration<CarrierTypeEF>
    {
        public void Configure(EntityTypeBuilder<CarrierTypeEF> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("carrier_type_id")
                .ValueGeneratedNever();

            builder.Property(e => e.Title)
                .IsRequired()
                .HasColumnName("carrier_type_name")
                .HasMaxLength(50);
        }
    }
}
