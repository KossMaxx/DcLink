using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductPicturesUrlConfiguration : IEntityTypeConfiguration<ProductPictureEF>
    {
        public void Configure(EntityTypeBuilder<ProductPictureEF> builder)
        {
            builder.HasKey(e => e.Id)
                   .HasName("PK_PicturesUrl1");

            builder.HasIndex(e => e.ProductId)
                .HasDatabaseName("tovID");

            builder.Property(e => e.Id).HasColumnName("picID");

            builder.Property(e => e.CobraPic).HasColumnName("cobra_pic");

            builder.Property(e => e.Date)
                .HasColumnName("ddd")
                .HasColumnType("datetime");

            builder.Property(e => e.ProductId).HasColumnName("tovID");

            builder.Property(e => e.Url)
                .IsRequired()
                .HasColumnName("url")
                .HasMaxLength(200);

            builder.Property(e => e.Uuu)
                .IsRequired()
                .HasColumnName("uuu")
                .HasMaxLength(20);
        }
    }
}
