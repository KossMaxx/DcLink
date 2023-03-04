using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductTypeCategoryParameterConfiguration : IEntityTypeConfiguration<ProductTypeCategoryParameterEF>
    {
        public void Configure(EntityTypeBuilder<ProductTypeCategoryParameterEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("param")
                .HasMaxLength(250);


            builder.Property(e => e.NameUA)
                .IsRequired()
                .HasColumnName("param_ua")
                .HasMaxLength(250);

            builder.Property(e => e.CategoryId).HasColumnName("katID");
        }
    }
}
