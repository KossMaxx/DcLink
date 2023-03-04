using System;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductRefundConfiguration : IEntityTypeConfiguration<ProductRefundEF>
    {
        public void Configure(EntityTypeBuilder<ProductRefundEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.ClientId).HasDatabaseName("IX_ПН_klient");
            builder.HasIndex(e => e.Date).HasDatabaseName("IX_ПН_data");
            builder.HasIndex(e => e.Type).HasDatabaseName("IX_ПН_type");
            
            builder.Property(e => e.Id).HasColumnName("НомерПН");
            builder.Property(e => e.Date).HasColumnName("Дата").HasColumnType("datetime");
            builder.Property(e => e.Type).HasColumnName("тип");
            builder.Property(e => e.ChangedAt).HasColumnName("LogTime").HasColumnType("datetime");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
        }
    }
}
