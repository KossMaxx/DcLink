using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductTypeCategoryConfiguration : IEntityTypeConfiguration<ProductTypeCategoryEF>
    {
        public void Configure(EntityTypeBuilder<ProductTypeCategoryEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("nazv")
                .HasMaxLength(50);
            builder.Property(e => e.TypeId).HasColumnName("tip");

            builder.Property(e => e.NameUA)
                .IsRequired()
                .HasColumnName("nazv_ua")
                .HasMaxLength(50);
            builder.Property(e => e.Web2).HasColumnName("web2");
            builder.Property(e => e.Web).HasColumnName("web");
            builder.Property(e => e.PriceTag).HasColumnName("Ценник");
            builder.Property(e => e.GroupId).HasColumnName("subID");

            builder.HasMany<ProductTypeCategoryParameterEF>(c => c.Parameters)
                .WithOne(p => p.Category).HasForeignKey(c => c.CategoryId);
            builder.HasOne(e => e.Group).WithOne().HasForeignKey<ProductTypeCategoryEF>(e => e.GroupId);
        }
    }
}
