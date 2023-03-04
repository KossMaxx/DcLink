using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class CompanyConfiguration :IEntityTypeConfiguration<CompanyEF>
    {
        public void Configure(EntityTypeBuilder<CompanyEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("ID");
            builder.Property(e => e.Title).HasColumnName("NAME_ukr");
            builder.Property(e => e.Okpo).HasColumnName("okpo");
        }
    }
}
