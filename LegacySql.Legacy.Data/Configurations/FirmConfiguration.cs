using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class FirmConfiguration : IEntityTypeConfiguration<FirmEF>
    {
        public void Configure(EntityTypeBuilder<FirmEF> builder)
        {
            builder.HasKey(e => e.Id).HasName("Код");
            builder.HasIndex(b => b.TaxCode).IsUnique();
            builder.HasIndex(b => b.Title).IsUnique();

            builder.Property(e => e.Id).HasColumnName("Код");
            builder.Property(e => e.TaxCode).HasColumnName("Окпо").HasMaxLength(10).IsRequired();
            builder.Property(e => e.Title).HasColumnName("Название").HasMaxLength(250).IsRequired();
            builder.Property(e => e.LegalAddress).HasColumnName("Адрес").HasMaxLength(250);
            builder.Property(e => e.Address).HasColumnName("AddressF").HasMaxLength(200);
            builder.Property(e => e.Phone).HasColumnName("Телефон").HasMaxLength(15);
            builder.Property(e => e.Account).HasColumnName("Рс").HasMaxLength(29);
            builder.Property(e => e.BankCode).HasColumnName("Мфо").HasMaxLength(6);
            builder.Property(e => e.BankName).HasColumnName("Банк").HasMaxLength(50);
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.LastChangeDate).HasColumnName("DataLastChange").HasColumnType("datetime");
            builder.Property(e => e.NotVat).HasColumnName("НеНДС");
            builder.Property(e => e.PayerCode).HasColumnName("Код_плат");
            builder.Property(e => e.CertificateNumber).HasColumnName("Номер_свид");
            builder.Property(e => e.IsNotResident).HasColumnName("is_not_resident");

            #region AutoGeneretedConfiguration

            // entity.HasKey(e => e.Код);
            //
            // entity.HasIndex(e => e.Название)
            //  .HasName("IX_Firms_1")
            //  .IsUnique();
            //
            // entity.HasIndex(e => e.Окпо)
            //  .HasName("IX_Firms")
            //  .IsUnique();
            //
            // entity.Property(e => e.AddressF).HasMaxLength(200);
            //
            // entity.Property(e => e.DataLastChange).HasColumnType("datetime");
            //
            // entity.Property(e => e.Email)
            //  .HasColumnName("email")
            //  .HasMaxLength(50);
            //
            // entity.Property(e => e.KlientId).HasColumnName("klientID");
            //
            // entity.Property(e => e.Passport).HasColumnName("passport");
            //
            // entity.Property(e => e.Pidstavadii)
            //  .HasColumnName("pidstavadii")
            //  .HasMaxLength(250);
            //
            // entity.Property(e => e.Адрес).HasMaxLength(250);
            //
            // entity.Property(e => e.Банк).HasMaxLength(50);
            //
            // entity.Property(e => e.Клиент).HasMaxLength(30);
            //
            // entity.Property(e => e.КодПлат)
            //  .HasColumnName("Код_плат")
            //  .HasMaxLength(12);
            //
            // entity.Property(e => e.Мфо)
            //  .HasColumnName("МФО")
            //  .HasMaxLength(6);
            //
            // entity.Property(e => e.Название)
            //  .IsRequired()
            //  .HasMaxLength(250);
            //
            // entity.Property(e => e.НеНдс).HasColumnName("НеНДС");
            //
            // entity.Property(e => e.НомерСвид)
            //  .HasColumnName("Номер_свид")
            //  .HasMaxLength(10);
            //
            // entity.Property(e => e.Окпо)
            //  .IsRequired()
            //  .HasColumnName("ОКПО")
            //  .HasMaxLength(10);
            //
            // entity.Property(e => e.Подпись).HasMaxLength(120);
            //
            // entity.Property(e => e.Рс).HasMaxLength(29);
            //
            // entity.Property(e => e.Телефон).HasMaxLength(15);

            #endregion
        }
    }
}
