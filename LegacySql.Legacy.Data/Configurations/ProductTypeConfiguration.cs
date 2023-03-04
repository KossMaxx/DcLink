using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductTypeEF>
    {
        public void Configure(EntityTypeBuilder<ProductTypeEF> builder)
        {

            builder.HasKey(e => e.Code);

            builder.Property(e => e.Code).HasColumnName("КодТипа");
            
            builder.Property(e => e.MainId).HasColumnName("mainID");

            builder.Property(e => e.Name).HasColumnName("НазваниеТипа").HasMaxLength(50);

            builder.Property(e => e.FullName).HasColumnName("ПолноеНазв").HasMaxLength(50);

            builder.Property(e => e.Web).HasColumnName("web").HasColumnType("bit");

            builder.Property(e => e.TypeNameUkr).HasColumnName("TypeNameUkr").HasMaxLength(50);

            builder.Property(e => e.LastChangeDate)
                .HasColumnName("DataLastChange")
                .HasColumnType("datetime");

            builder.Property(e => e.Status).HasColumnName("type_status").HasMaxLength(50);
        }
    }
}
