using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<CountryEF>
    {
        public void Configure(EntityTypeBuilder<CountryEF> builder)
        {
            builder.HasKey(e => e.Id)
                    .HasName("TBL_CountryNames_PK");

            builder.Property(e => e.Id).HasColumnName("CN_ID");

            builder.Property(e => e.IsoId).HasColumnName("C_ID");

            builder.Property(e => e.Title)
                .IsRequired()
                .HasColumnName("CN_Name")
                .HasMaxLength(150);

            builder.Property(e => e.LanguageId).HasColumnName("L_ID");
        }
    }
}
