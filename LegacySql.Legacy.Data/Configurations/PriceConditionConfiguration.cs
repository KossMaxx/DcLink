using System;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class PriceConditionConfiguration : IEntityTypeConfiguration<PriceConditionEF>
    {
        public void Configure(EntityTypeBuilder<PriceConditionEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => new {e.ClientId, e.Vendor}).HasDatabaseName("klientID");
            builder.HasIndex(e => new {e.ClientId, e.ProductTypeId}).HasDatabaseName("klientID-tip");
            
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.Date).HasColumnName("date_stamp").HasColumnType("datetime");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.ProductTypeId).HasColumnName("tip");
            builder.Property(e => e.Vendor).HasColumnName("vendor").HasMaxLength(20);
            builder.Property(e => e.ProductManager).HasColumnName("product").HasMaxLength(10);
            builder.Property(e => e.PriceType).HasColumnName("kolonka");
            builder.Property(e => e.DateTo).HasColumnName("validdate");
            builder.Property(e => e.Comment).HasColumnName("prim").HasMaxLength(50);
            builder.Property(e => e.Value).HasColumnName("value").HasColumnType("money");
            builder.Property(e => e.PercentValue).HasColumnName("percent_value").HasColumnType("money");
            builder.Property(e => e.UpperThresholdPriceType).HasColumnName("upper_threshold_column");
        }
    }
}
