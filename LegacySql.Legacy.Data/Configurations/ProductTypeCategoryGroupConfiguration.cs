using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductTypeCategoryGroupConfiguration : IEntityTypeConfiguration<ProductTypeCategoryGroupEF>
    {
        public void Configure(EntityTypeBuilder<ProductTypeCategoryGroupEF> builder)
        {

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("ID");
            builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
            builder.Property(e => e.NameUA).HasColumnName("name_ua").HasMaxLength(50);
            builder.Property(e => e.Sort).HasColumnName("sort").HasColumnType("tinyint");
        }
    }
}