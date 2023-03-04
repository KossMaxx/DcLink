using LegacySql.Legacy.Data.Models;using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class RelatedProductConfiguration : IEntityTypeConfiguration<RelatedProductEF>
    {
        public void Configure(EntityTypeBuilder<RelatedProductEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("RgoodsID");
            builder.Property(e => e.MainProductId).HasColumnName("tovar1");
            builder.Property(e => e.RelatedProductId).HasColumnName("tovar2");
            builder.Property(e => e.LogDate).HasColumnName("logtime").HasColumnType("datetime");
        }
    }
}
