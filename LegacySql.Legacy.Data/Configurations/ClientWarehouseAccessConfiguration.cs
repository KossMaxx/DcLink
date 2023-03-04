using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ClientWarehouseAccessConfiguration : IEntityTypeConfiguration<ClientWarehouseAccessEF>
    {
        public void Configure(EntityTypeBuilder<ClientWarehouseAccessEF> builder)
        {
            builder.HasKey(e => e.Id).HasName("id");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.WarehouseId).HasColumnName("sklad");
            builder.Property(e => e.HasAccess).HasColumnName("price");
        }
    }
}
