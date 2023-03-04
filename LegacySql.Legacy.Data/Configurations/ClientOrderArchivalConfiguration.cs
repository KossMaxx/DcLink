using System;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ClientOrderArchivalConfiguration : IEntityTypeConfiguration<ClientOrderArchivalEF>
    {
        public void Configure(EntityTypeBuilder<ClientOrderArchivalEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Date)
                .HasDatabaseName("IX_РН_data");
            builder.HasIndex(e => e.ClientId)
                .HasDatabaseName("IX_РН_klient");
            builder.HasIndex(e => new { e.Id, e.Date, e.ClientId, IsActive = e.IsExecuted })
                .HasDatabaseName("РН_ф");
            builder.Property(e => e.Id).HasColumnName("НомерПН");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.Date).HasColumnName("Дата").HasColumnType("datetime");
            builder.Property(e => e.Comments).HasColumnName("Описание").HasMaxLength(250);
            builder.Property(e => e.ChangedAt)
                    .HasColumnName("modified_at")
                    .HasColumnType("datetime");
            builder.Property(e => e.IsExecuted).HasColumnName("ф");
            builder.Property(e => e.IsCashless).HasColumnName("isCashless");
            builder.Property(e => e.MarketplaceNumber)
                .HasColumnName("customer_order_ID")
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.Manager)
                .HasColumnName("менеджер")
                .HasMaxLength(30);
            builder.Property(e => e.IsPaid).HasColumnName("Paid");
            builder.Property(e => e.WarehouseId).HasColumnName("Отдел");
            builder.Property(e => e.PaymentDate).HasColumnName("DataBal").HasColumnType("datetime");
        }
    }
}
