using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<LanguageEF>
    {
        public void Configure(EntityTypeBuilder<LanguageEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.Title)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(50);
        }
    }
}
