using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductPriceConditionConfiguration : IEntityTypeConfiguration<ProductPriceConditionEF>
    {
        public void Configure(EntityTypeBuilder<ProductPriceConditionEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.ClientId).HasDatabaseName("<tovarID and price>");
            builder.HasIndex(e => new {e.ClientId, e.ProductId}).HasDatabaseName("KlienID_TovarID");
            
            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.ProductId).HasColumnName("tovarID");
            builder.Property(e => e.Price).HasColumnName("price");
            builder.Property(e => e.DateTo).HasColumnName("validdate");
            builder.Property(e => e.Value).HasColumnName("price_value").HasColumnType("money");
            builder.Ignore(e => e.Currency);
        }
    }
}
