using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class DeliveryTypeConfiguration : IEntityTypeConfiguration<DeliveryTypeEF>
    {
        public void Configure(EntityTypeBuilder<DeliveryTypeEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("DistavkaTipID");
            builder.Property(e => e.CarrierTypeId).HasColumnName("carrier_type");
            builder.Property(e => e.Title).HasColumnName("Nazv").HasMaxLength(50);

            #region AutoGeneretedConfiguration

            //entity.Property(e => e.AllowUse).HasColumnName("allow_use");

            //entity.Property(e => e.CatId).HasColumnName("cat_id");

            //entity.Property(e => e.HasInvoice).HasColumnName("hasInvoice");

            //entity.Property(e => e.Web).HasColumnName("web");

            //entity.Property(e => e.WebName)
            //    .HasColumnName("webName")
            //    .HasMaxLength(50);

            #endregion
        }
    }
}
