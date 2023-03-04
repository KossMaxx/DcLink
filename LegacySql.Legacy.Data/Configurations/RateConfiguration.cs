using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LegacySql.Legacy.Data.Configurations
{
    public class RateConfiguration : IEntityTypeConfiguration<RateEF>
    {
        public void Configure(EntityTypeBuilder<RateEF> builder)
        {
            builder.HasKey(e => e.Id).HasName("rateID");

            builder.Property(e => e.Id).HasColumnName("rateID");
            builder.Property(e => e.Title).HasColumnName("nazv");
            builder.Property(e => e.Value).HasColumnName("rate");
        }
    }
}
