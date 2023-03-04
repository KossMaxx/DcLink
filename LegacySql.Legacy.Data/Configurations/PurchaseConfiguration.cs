using System;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<PurchaseEF>
    {
        public void Configure(EntityTypeBuilder<PurchaseEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.ClientId).HasDatabaseName("IX_ПН_klient");
            builder.HasIndex(e => e.Date).HasDatabaseName("IX_ПН_data");
            builder.HasIndex(e => e.Type).HasDatabaseName("IX_ПН_type");
            builder.HasIndex(e => e.IsExecuted).HasDatabaseName("PN_F");
            builder.HasIndex(e => new { TransactionProcessed = e.IsExecuted, e.IsProductsArrivedToPort }).HasDatabaseName("ф__tranzit");
            
            builder.Property(e => e.Id).HasColumnName("НомерПН");
            builder.Property(e => e.Date).HasColumnName("Дата").HasColumnType("datetime");
            builder.Property(e => e.IsExecuted).HasColumnName("ф");
            builder.Property(e => e.Comments).HasColumnName("Описание").HasMaxLength(500);
            builder.Property(e => e.Type).HasColumnName("тип");
            builder.Property(e => e.IsActual).HasColumnName("ok");
            builder.Property(e => e.TransportationCost).HasColumnName("Поле85").HasColumnType("money");
            builder.Property(e => e.CostType).HasColumnName("dCostTip");
            builder.Property(e => e.IsApproved).HasColumnName("test");
            builder.Property(e => e.IsFinancialSideConfirmed).HasColumnName("balOk");
            builder.Property(e => e.IsProductsArrivedToPort).HasColumnName("tranzit");
            builder.Property(e => e.IsCashlessDocumentsProcessNeeded).HasColumnName("need_bn");
            builder.Property(e => e.SupplierDocument).HasColumnName("supl_doc_ID").HasMaxLength(50);
            builder.Property(e => e.ShippingDate).HasColumnName("date_shipping").HasColumnType("datetime");
            builder.Property(e => e.ChangedAt).HasColumnName("LogTime").HasColumnType("datetime");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.WarehouseId).HasColumnName("Отдел");
            builder.Property(e => e.IsPaid).HasColumnName("Paid");
            builder.Property(e => e.EmployeeUsername).HasColumnName("окюзер");
            builder.Property(e => e.PaymentDate).HasColumnName("DataBal");
        }
    }
}
