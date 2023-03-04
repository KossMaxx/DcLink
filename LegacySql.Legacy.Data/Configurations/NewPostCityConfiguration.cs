using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class NewPostCityConfiguration : IEntityTypeConfiguration<NewPostCityEF>
    {
        public void Configure(EntityTypeBuilder<NewPostCityEF> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.Description, e.CityRef })
                .HasDatabaseName("NP_Cities_Ref_DescriptionRu");

            builder.Property(e => e.Id).HasColumnName("counterID");
            builder.Property(e => e.Description)
                .HasColumnName("DescriptionRu")
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(e => e.CityRef)
                .HasColumnName("Ref")
                .IsRequired();

            #region AutoGeneretedConfiguration

            //entity.Property(e => e.CityId).HasColumnName("CityID");

            //entity.Property(e => e.Conglomerates).HasMaxLength(50);

            //entity.Property(e => e.Description)
            //    .IsRequired()
            //    .HasMaxLength(50);

            //entity.Property(e => e.SettlementTypeDescription).HasMaxLength(50);

            //entity.Property(e => e.SettlementTypeDescriptionRu).HasMaxLength(50);

            #endregion
        }
    }
}
