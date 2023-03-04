using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<DepartmentEF>
    {
        public void Configure(EntityTypeBuilder<DepartmentEF> builder)
        {
            builder.ToTable("departments");
            
            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Title)
                .HasColumnName("nazv")
                .HasMaxLength(50);

            builder.Property(e => e.Description)
                .HasColumnName("descr")
                .HasMaxLength(200);

            builder.Property(e => e.BossPosition)
                .HasColumnName("boss")
                .HasMaxLength(50);

            builder.Property(e => e.BossId)
                .HasColumnName("BossId");
        }
    }
}
