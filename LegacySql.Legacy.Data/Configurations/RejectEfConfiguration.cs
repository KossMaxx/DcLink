using System;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class RejectConfiguration : IEntityTypeConfiguration<RejectEF>
    {
        public void Configure(EntityTypeBuilder<RejectEF> builder)
        {
            builder.HasKey(e => e.Id).HasName("Brak_ID");
            builder.Property(e => e.Id).HasColumnName("Brak_ID");

            builder.Property(e => e.CreatedAt).HasColumnName("date_created");
            builder.Property(e => e.Date).HasColumnName("d1");
            builder.Property(e => e.SerialNumber).HasColumnName("sernom").HasMaxLength(50);
            builder.Property(e => e.ClientTitle).HasColumnName("klient").HasMaxLength(30);
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.HasIndex(e => e.ClientId).HasDatabaseName("IX_brak_klient");
            builder.Property(e => e.StatusForClient).HasColumnName("tip_zakr");
            builder.Property(e => e.WarehouseId).HasColumnName("sklad");
            builder.Property(e => e.ResponsibleForStatus).HasColumnName("sozdal").HasMaxLength(20).IsUnicode(false);
            builder.Property(e => e.RepairType).HasColumnName("rma_type");
            builder.Property(e => e.DefectDescription).HasColumnName("akt_defect").HasMaxLength(500);
            builder.Property(e => e.KitDescription).HasColumnName("Комплектность").HasMaxLength(200);
            builder.Property(e => e.ProductStatusDescription).HasColumnName("descr").HasMaxLength(250);
            builder.Property(e => e.Notes).HasColumnName("прим").HasMaxLength(150);
            builder.Property(e => e.ProductStatus).HasColumnName("condition").HasMaxLength(200);
            builder.Property(e => e.ClientOrderId).HasColumnName("rn_id");
            builder.Property(e => e.ClientOrderDate).HasColumnName("d2");
            builder.Property(e => e.ReceiptDocumentDate).HasColumnName("dpokupki");
            builder.Property(e => e.ReceiptDocumentId).HasColumnName("partner_doc_ID").HasMaxLength(50);
            builder.Property(e => e.SupplierId).HasColumnName("suplID");
            builder.Property(e => e.SupplierTitle).HasColumnName("supl1").HasMaxLength(30);
            builder.Property(e => e.PurchasePrice).HasColumnName("cost2").HasColumnType("money");
            builder.Property(e => e.ProductMark).HasColumnName("Марка").HasMaxLength(250);
            builder.Property(e => e.ProductId).HasColumnName("Кодтовара");
            builder.Property(e => e.ProductTypeId).HasColumnName("код_типа");
            builder.Property(e => e.PurchaseCurrencyPrice).HasColumnName("cost1").HasColumnType("money");
            builder.Property(e => e.OutgoingWarranty).HasColumnName("war_ost_klient");
            builder.Property(e => e.DepartureDate).HasColumnName("d_otpr");           
            builder.Property(e => e.ChangedAt).HasColumnName("LogTime").HasColumnType("datetime");
            builder.Property(e => e.StatusForService).HasColumnName("tip_vozvr");
            builder.Property(e => e.ProductRefundId).HasColumnName("pn_id");
            builder.Property(e => e.SupplierProductId).HasColumnName("Кодтовара2");
            builder.Property(e => e.SupplierDescription).HasColumnName("descrSupl");
            builder.Property(e => e.ReturnDate).HasColumnName("d_vozvr");
            builder.Property(e => e.SupplierProductMark).HasColumnName("marka2");
            builder.Property(e => e.SupplierSerialNumber).HasColumnName("sernom2");

            builder.Ignore(e => e.Amount);
            builder.Ignore(e => e.BuyDocDate);
            builder.Ignore(e => e.SellDocDate);
        }
    }
}
