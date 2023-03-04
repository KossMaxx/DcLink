using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ClientDeliveryAddressConfiguration : IEntityTypeConfiguration<ClientDeliveryAddressEF>
    {
        public void Configure(EntityTypeBuilder<ClientDeliveryAddressEF> builder)
        {
            builder.HasKey(e => e.Id).HasName("shipping_addr_ID");

            builder.Property(e => e.Id).HasColumnName("shipping_addr_ID");
            builder.Property(e => e.Address).HasColumnName("dostavkaAdr").HasMaxLength(150);
            builder.Property(e => e.ContactPerson).HasColumnName("dostavkaFIO").HasMaxLength(150);
            builder.Property(e => e.Phone).HasColumnName("dostavkaTel").HasMaxLength(50);
            builder.Property(e => e.WaybillAddress).HasColumnName("WayBIll_addr").HasMaxLength(100);
            builder.Property(e => e.Type).HasColumnName("addr_type");
            builder.Property(e => e.ClientId).HasColumnName("client_ID");
        }
    }
}
