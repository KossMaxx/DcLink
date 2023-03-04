using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class CountryIsoCodeConfiguration : IEntityTypeConfiguration<CountryIsoCodeEF>
    {
        public void Configure(EntityTypeBuilder<CountryIsoCodeEF> builder)
        {
            builder.HasKey(e => e.Id)
                    .HasName("TBL_Countries_PK");

            builder.Property(e => e.Id).HasColumnName("C_ID");

            builder.Property(e => e.Code)
                .IsRequired()
                .HasColumnName("C_ISO2_code")
                .HasMaxLength(2)
                .IsUnicode(false);
        }
    }
}
