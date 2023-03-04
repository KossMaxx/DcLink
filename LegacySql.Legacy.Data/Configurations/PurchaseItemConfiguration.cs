using System;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItemEF>
    {
        public void Configure(EntityTypeBuilder<PurchaseItemEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Quantity).HasDatabaseName("Приход_Количество");
            builder.HasIndex(e => e.PurchaseId).HasDatabaseName("IX_Приход_PN");
            builder.HasIndex(e => e.ProductId).HasDatabaseName("IX_Приход_tovID");
            
            builder.Property(e => e.Id).HasColumnName("КодОперации");
            builder.Property(e => e.Quantity).HasColumnName("Количество");
            builder.Property(e => e.Price).HasColumnName("Цена").HasColumnType("money");
            builder.Property(e => e.PurchaseId).HasColumnName("НомерПН");
            builder.Property(e => e.ProductId).HasColumnName("КодТовара");
        }
    }
}
