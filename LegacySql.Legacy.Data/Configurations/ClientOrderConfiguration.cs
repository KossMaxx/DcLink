using System;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ClientOrderConfiguration : IEntityTypeConfiguration<ClientOrderEF>
    {
        public void Configure(EntityTypeBuilder<ClientOrderEF> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Date)
                .HasDatabaseName("IX_РН_data");
            builder.HasIndex(e => e.ClientId)
                .HasDatabaseName("IX_РН_klient");
            builder.HasIndex(e => new { e.Id, e.Date, e.ClientId, IsActive = e.IsExecuted })
                .HasDatabaseName("РН_ф");
            builder.HasIndex(e => e.PaymentDate);

            builder.Property(e => e.Id).HasColumnName("НомерПН");
            builder.Property(e => e.ClientId).HasColumnName("klientID");
            builder.Property(e => e.Date).HasColumnName("DataSozd").HasColumnType("datetime");
            builder.Property(e => e.Comments).HasColumnName("Описание").HasMaxLength(250);
            builder.Property(e => e.ChangedAt)
                    .HasColumnName("modified_at")
                    .HasColumnType("datetime");
            builder.Property(e => e.IsExecuted).HasColumnName("ф");
            builder.Property(e => e.IsCashless).HasColumnName("isCashless");
            builder.Property(e => e.MarketplaceNumber)
                .HasColumnName("customer_order_ID")
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.Manager)
                .HasColumnName("менеджер")
                .HasMaxLength(30);
            builder.Property(e => e.IsPaid).HasColumnName("Paid");
            builder.Property(e => e.WarehouseId).HasColumnName("Отдел");
            builder.Property(e => e.Amount).HasColumnName("Сумма");
            builder.Property(e => e.Quantity).HasColumnName("Кол");
            builder.Property(e => e.PaymentDate).HasColumnName("DataBal").HasColumnType("datetime");

            #region AutoGeneretedConfiguration

            //entity.HasIndex(e => e.Колонка)
            //    .HasName("IX_РН_Column");

            //entity.HasIndex(e => new { e.Дата, e.DataBal, e.Ф, e.KlientId })
            //    .HasName("ф_klientID>Дата_DataBal");

            //entity.HasIndex(e => new { e.НомерПн, e.DataBal, e.KlientId, e.Ф })
            //    .HasName("РН_НомерПН_DataBal_klientID");

            //entity.HasIndex(e => new { e.НомерПн, e.Profit, e.Ф, e.Paid })
            //    .HasName("<РН_f_paid>");

            //entity.HasIndex(e => new { e.НомерПн, e.Отдел, e.Ф, e.Дата })
            //    .HasName("RN_f_Data");

            //entity.HasIndex(e => new { e.НомерПн, e.СуммаЗ, e.Ф, e.Paid })
            //    .HasName("<РН_СуммаЗ_ф_Paid>");

            //entity.HasIndex(e => new { e.НомерПн, e.Ф, e.Paid, e.Profit })
            //    .HasName("<[РН_f_paid_Profit>");

            //entity.HasIndex(e => new { e.НомерПн, e.KlientId, e.Ф, e.Дата, e.Колонка })
            //    .HasName("RN_F_Data_kolonka");

            //entity.HasIndex(e => new { e.Сумма, e.Отдел, e.KlientId, e.Ф, e.Дата })
            //    .HasName("РН_ф_Дата");

            //entity.HasIndex(e => new { e.НомерПн, e.СуммаЗ, e.KlientId, e.Ф, e.Paid, e.Сумма })
            //    .HasName("RN_f_Paid_Summa");

            //entity.HasIndex(e => new { e.НомерПн, e.Ф, e.Отдел, e.Разрешил, e.Expected, e.Д2 })
            //    .HasName("РН_д2");

            //entity.HasIndex(e => new { e.НомерПн, e.Накладная, e.Клиент, e.Сумма, e.СуммаЗ, e.Кол, e.Курс, e.Ф, e.Д1, e.Д2, e.Описание, e.Менеджер, e.Отдел, e.Подпись, e.СерияД, e.НомерД, e.ДатаД, e.War, e.Колонка, e.Разрешил, e.Razreshil, e.Vidal, e.Rezervdata, e.Lastuser, e.Lasttime, e.Типдок, e.Sozdal, e.Dsozd, e.Sborka, e.Gotovo, e.Sobral, e.DataBal, e.DataSozd, e.SborkaN, e.Pidstava, e.Naotgruzku, e.Expected, e.Ftime, e.Fio, e.Zamok, e.Zapas, e.BalOk, e.BalOkuser, e.KlientId, e.WebUserId, e.Z, e.Paid, e.PaidDate, e.Nobonus, e.KolZ, e.Profit, e.CostsFd, e.CostsDostavka, e.FinregistratorId, e.Fiscalebillprinted, e.CustomerOrderId, e.CustomerSumm, e.Block, e.BlockUser, e.FPart, e.Дата })
            //    .HasName("РН_ДАТА");

            //entity.Property(e => e.BalOk).HasColumnName("balOK");

            //entity.Property(e => e.BalOkuser)
            //    .HasColumnName("balOKuser")
            //    .HasMaxLength(20)
            //    .IsFixedLength();

            //entity.Property(e => e.Block).HasColumnName("block");

            //entity.Property(e => e.BlockUser)
            //    .HasColumnName("block_user")
            //    .HasMaxLength(10)
            //    .IsFixedLength();

            //entity.Property(e => e.CostsDostavka)
            //    .HasColumnName("costsDostavka")
            //    .HasColumnType("money");

            //entity.Property(e => e.CostsFd)
            //    .HasColumnName("costsFD")
            //    .HasColumnType("money");


            //entity.Property(e => e.CustomerSumm)
            //    .HasColumnName("customer_summ")
            //    .HasColumnType("money");

            //entity.Property(e => e.DataSozd).HasColumnType("datetime");

            //entity.Property(e => e.Dsozd)
            //    .HasColumnName("dsozd")
            //    .HasColumnType("datetime");

            //entity.Property(e => e.Expected).HasColumnName("expected");

            //entity.Property(e => e.FPart).HasColumnName("f_part");

            //entity.Property(e => e.FinregistratorId).HasColumnName("finregistratorID");

            //entity.Property(e => e.Fio)
            //    .HasColumnName("FIO")
            //    .HasMaxLength(50);

            //entity.Property(e => e.Fiscalebillprinted).HasColumnName("fiscalebillprinted");

            //entity.Property(e => e.Ftime).HasColumnType("datetime");

            //entity.Property(e => e.Gotovo).HasColumnName("gotovo");

            //entity.Property(e => e.Lasttime)
            //    .HasColumnName("lasttime")
            //    .HasColumnType("datetime");

            //entity.Property(e => e.Lastuser)
            //    .HasColumnName("lastuser")
            //    .HasMaxLength(20)
            //    .IsFixedLength();

            //entity.Property(e => e.Naotgruzku)
            //    .HasColumnName("naotgruzku")
            //    .HasMaxLength(20);

            //entity.Property(e => e.Nobonus).HasColumnName("nobonus");

            //entity.Property(e => e.PaidDate).HasColumnType("date");

            //entity.Property(e => e.Pidstava)
            //    .HasColumnName("pidstava")
            //    .HasMaxLength(50);

            //entity.Property(e => e.Profit).HasColumnType("money");

            //entity.Property(e => e.Razreshil)
            //    .HasColumnName("razreshil")
            //    .HasMaxLength(20);

            //entity.Property(e => e.Rezervdata)
            //    .HasColumnName("REZERVDATA")
            //    .HasColumnType("datetime");

            //entity.Property(e => e.Sborka).HasColumnName("sborka");

            //entity.Property(e => e.SborkaN).HasColumnName("sborkaN");

            //entity.Property(e => e.Sobral)
            //    .HasColumnName("sobral")
            //    .HasMaxLength(20);

            //entity.Property(e => e.Sozdal)
            //    .HasColumnName("sozdal")
            //    .HasMaxLength(20)
            //    .IsFixedLength();

            //entity.Property(e => e.Vidal)
            //    .HasColumnName("vidal")
            //    .HasMaxLength(20);

            //entity.Property(e => e.War).HasColumnName("war");

            //entity.Property(e => e.WebUserId).HasColumnName("WebUserID");

            //entity.Property(e => e.Z).HasColumnName("z");

            //entity.Property(e => e.Zamok)
            //    .HasColumnName("zamok")
            //    .HasMaxLength(20);

            //entity.Property(e => e.Zapas)
            //    .HasColumnName("zapas")
            //    .HasColumnType("money");

            //entity.Property(e => e.Д1).HasColumnName("д1");

            //entity.Property(e => e.Д2).HasColumnName("д2");

            //entity.Property(e => e.ДатаД)
            //    .HasColumnName("датаД")
            //    .HasColumnType("datetime");

            //entity.Property(e => e.Клиент).HasMaxLength(30);

            //entity.Property(e => e.Колонка).HasColumnName("колонка");

            //entity.Property(e => e.Курс).HasColumnName("курс");

            //entity.Property(e => e.Менеджер)
            //    .HasColumnName("менеджер")
            //    .HasMaxLength(30);

            //entity.Property(e => e.Накладная).HasColumnName("накладная");

            //entity.Property(e => e.НомерД)
            //    .HasColumnName("номерД")
            //    .HasMaxLength(13);

            //entity.Property(e => e.Подпись)
            //    .HasColumnName("подпись")
            //    .HasMaxLength(50);

            //entity.Property(e => e.Разрешил).HasColumnName("разрешил");

            //entity.Property(e => e.СерияД)
            //    .HasColumnName("серияД")
            //    .HasMaxLength(5);

            //entity.Property(e => e.Типдок).HasColumnName("типдок");

            #endregion
        }
    }
}
