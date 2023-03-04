using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductGeneralConfiguration : IEntityTypeConfiguration<ProductGeneralEF>
    {
        public void Configure(EntityTypeBuilder<ProductGeneralEF> builder)
        {
            builder.HasKey(e => e.Code)
                .HasName("PK_Товары3");

            builder.HasIndex(e => e.Brand)
                .HasDatabaseName("Марка");

            builder.HasIndex(e => e.WorkName)
                .HasDatabaseName("Позиция");

            builder.HasIndex(e => e.NonCashProductId)
                .HasDatabaseName("Item_beznal_tovID");

            builder.Property(e => e.VendorCode)
                .HasColumnName("artikul")
                .HasMaxLength(48);

            builder.Property(e => e.NameForPrinting)
                .HasColumnName("beznal")
                .HasMaxLength(300);

            builder.Property(e => e.Code).HasColumnName("КодТовара");
            builder.Property(e => e.WorkName).HasMaxLength(400).HasColumnName("Позиция");
            builder.Property(e => e.ProductTypeId).HasColumnName("КодТипа");

            builder.Property(e => e.IsProductIssued).HasColumnName("contentOK");

            builder.Property(e => e.ContentUser)
                .HasColumnName("contentUser")
                .HasMaxLength(20);

            builder.Property(e => e.ProductCountryId).HasColumnName("countryOfOrigin_ID");

            builder.Property(e => e.BrandCountryId).HasColumnName("countryOfRegistration_ID");
            builder.Property(e => e.NomenclatureBarcode)
                .HasColumnName("EAN")
                .HasMaxLength(50);

            builder.Property(e => e.Game).HasColumnName("game");


            builder.Property(e => e.IsDistribution).HasColumnName("is_distribution");

            builder.Property(e => e.ProductCategory)
                .HasColumnName("klas")
                .HasMaxLength(30);

            builder.Property(e => e.NomenclatureCode)
                .HasColumnName("KodZED")
                .HasMaxLength(30);

            builder.Property(e => e.PackageQuantity).HasColumnName("kolpak");

            builder.Property(e => e.ManualRrp).HasColumnName("manual_rrp");

            builder.Property(e => e.Manufacture)
                .HasColumnName("manufacture")
                .HasMaxLength(20);
            builder.Property(e => e.Price).HasColumnName("PRICE");

            builder.Property(e => e.NotInvolvedInPricing).HasColumnName("pricealgoritm_ignore");
            builder.Property(e => e.Monitoring).HasColumnName("RRP");
            builder.Property(e => e.ScanHotline).HasColumnName("scan_hotline");

            builder.Property(e => e.ScanMonitoring).HasColumnName("scan_monitoring");
            builder.Property(e => e.Vat)
                .HasColumnName("VAT")
                .HasColumnType("money");

            builder.Property(e => e.VideoUrl)
                .HasColumnName("videoURL")
                .HasMaxLength(100);

            builder.Property(e => e.Guarantee).HasColumnName("war");

            builder.Property(e => e.GuaranteeIn).HasColumnName("warin");

            builder.Property(e => e.Weight).HasColumnName("вес");

            builder.Property(e => e.Unit)
                .HasColumnName("изм")
                .HasMaxLength(8);

            builder.Property(e => e.Brand).HasMaxLength(300).HasColumnName("Марка");

            builder.Property(e => e.Pending).HasColumnName("нал_ожид");

            builder.Property(e => e.InReserve).HasColumnName("нал_резерв");

            builder.Property(e => e.InStock).HasColumnName("нал_ф");

            builder.Property(e => e.IsImported).HasColumnName("о_импрот");

            builder.Property(e => e.Volume).HasColumnName("обьем");

            builder.Property(e => e.Subtype)
                .HasColumnName("подтип")
                .HasMaxLength(40);

            builder.Property(e => e.Markdown).HasColumnName("уценка");

            builder.Property(e => e.LastChangeDate)
                    .HasColumnName("DataLastChange")
                    .HasColumnType("datetime");

            builder.Property(e => e.NonCashProductId).HasColumnName("beznal_tovID");

            builder.Property(e => e.Currency).HasColumnName("ВалютаТовара").HasColumnType("tinyint");
        }
    }
}
