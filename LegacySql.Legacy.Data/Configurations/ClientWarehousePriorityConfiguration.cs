using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ClientWarehousePriorityConfiguration : IEntityTypeConfiguration<ClientWarehousePriorityEF>
    {
        public void Configure(EntityTypeBuilder<ClientWarehousePriorityEF> builder)
        {
            builder.HasKey(e => e.Id).HasName("id");

            builder.Property(e => e.Id).HasColumnName("id");
            builder.Property(e => e.ClientId).HasColumnName("client_id");
            builder.Property(e => e.WarehouseId).HasColumnName("sklad_id");
            builder.Property(e => e.Priority).HasColumnName("order_index");
        }
    }
}
