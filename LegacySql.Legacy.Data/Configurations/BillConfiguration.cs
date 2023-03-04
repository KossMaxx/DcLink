using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class BillConfiguration : IEntityTypeConfiguration<BillEF>
    {
        public void Configure(EntityTypeBuilder<BillEF> builder)
        {
            builder.Property(e => e.Id).HasColumnName("Код_счета");
            builder.Property(e => e.Number).HasColumnName("Номер");
            builder.Property(e => e.Issued).HasColumnName("выдан");
            builder.Property(e => e.Date).HasColumnName("дата");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.Comments).HasColumnName("Описание");
            builder.Property(e => e.ChangedAt).HasColumnName("DataLastChange");
            builder.Property(e => e.Seller).HasColumnName("OOO");
            builder.Property(e => e.ValidToDate).HasColumnName("ValidTo");
            builder.Property(e => e.FirmId).HasColumnName("FirmID");
            builder.Property(e => e.Creator).HasColumnName("юзер");
            builder.Property(e => e.ManagerId).HasColumnName("Manager");
            builder.Property(e => e.IsPaid).HasColumnName("оплачен");
            builder.Property(e => e.Rate).HasColumnName("курс");
        }
    }
}
