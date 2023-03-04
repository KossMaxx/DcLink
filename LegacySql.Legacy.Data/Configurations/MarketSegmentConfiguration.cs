using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class MarketSegmentConfiguration : IEntityTypeConfiguration<MarketSegmentEF>
    {
        public void Configure(EntityTypeBuilder<MarketSegmentEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Title).HasColumnName("name");
        }
    }
}
