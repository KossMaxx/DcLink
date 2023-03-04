using LegacySql.Legacy.Data.Models;using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class PhysicalPersonConfiguration : IEntityTypeConfiguration<PhysicalPersonEF>
    {
        public void Configure(EntityTypeBuilder<PhysicalPersonEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("КодСотрудника");
            builder.Property(e => e.FirstName).HasColumnName("Имя").HasMaxLength(50);
            builder.Property(e => e.LastName).HasColumnName("Фамилия").HasMaxLength(50);
            builder.Property(e => e.Fired).HasColumnName("уволен");
            builder.Property(e => e.TechnicalBalance).HasColumnName("technical_balance");
            builder.Property(e => e.JobPosition).HasColumnName("Должность").HasMaxLength(50);
            builder.Property(e => e.WorkPhone).HasColumnName("РабочийТелефон").HasMaxLength(30);
            builder.Property(e => e.PassportSerialNumber).HasColumnName("паспорт").HasMaxLength(50);
            builder.Property(e => e.PassportIssuer).HasColumnName("выдан").HasMaxLength(60);
            builder.Property(e => e.PassportSeries).HasColumnName("серия").HasMaxLength(50);
            builder.Property(e => e.PassportIssuedAt).HasColumnName("выданД");
            builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(50);
            builder.Ignore(e => e.ChangedAt);
            builder.Property(e => e.NickName).HasColumnName("uuu").HasMaxLength(20);
            builder.Property(e => e.FullName).HasColumnName("ФИОполн").HasMaxLength(50);
            builder.Property(e => e.MiddleName).HasColumnName("Отчество").HasMaxLength(50);
            builder.Property(e => e.IndividualTaxNumber).HasColumnName("INN").HasMaxLength(10);
            builder.Property(e => e.InternalPhone).HasColumnName("Внутренний").HasMaxLength(30);
        }
    }
}
