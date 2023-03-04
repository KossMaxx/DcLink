using System;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ClientOrderItemConfiguration : IEntityTypeConfiguration<ClientOrderItemEF>
    {
        public void Configure(EntityTypeBuilder<ClientOrderItemEF> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.NomenclatureId)
                .HasDatabaseName("IX_Расход_tovID");

            builder.HasIndex(e => e.ClientOrderId)
                .HasDatabaseName("IX_Расход_RN");

            builder.Property(e => e.Id).HasColumnName("КодОперации");
            builder.Property(e => e.ClientOrderId).HasColumnName("НомерПН");
            builder.Property(e => e.NomenclatureId).HasColumnName("КодТовара");
            builder.Property(e => e.Quantity).HasColumnName("КолЗ");
            builder.Property(e => e.ChangedAt)
                    .HasColumnName("modified_at")
                    .HasColumnType("datetime");
            builder.Property(e => e.Price)
                .HasColumnName("Цена")
                .HasColumnType("money")
                .HasComment("Цена в валюте документа");

            builder.Property(e => e.PriceUAH)
                .HasColumnName("ЦенаГРН")
                .HasColumnType("money");

            builder.Property(e => e.Warranty).HasColumnName("warranty");

            builder.HasMany(e => e.SerialNumbers)
                .WithOne(e => e.ClientOrderItem)
                .HasForeignKey(e => e.ClientOrderItemId);

            #region AutoGeneretedConfiguration

            //entity.HasIndex(e => new {e.НомерПн, e.КодТовара, e.Цена, e.Количество, e.Crosskurs})
            //    .HasName("Расход_crosskurs");

            //entity.HasIndex(e => new {e.КодОперации, e.НомерПн, e.КодТипа, e.КодТовара, e.Количество, e.КолСв, e.КолЗ})
            //    .HasName("<Расход_КолЗ");

            //entity.HasIndex(e => new {e.НомерПн, e.Цена, e.Количество, e.Ss, e.Exps, e.КодТовара, e.Crosskurs})
            //    .HasName("Rashod_kodtovara_kroskurs");

            //entity.HasIndex(e => new
            //    {
            //        e.КодОперации, e.КодТипа, e.КодТовара, e.Марка, e.Цена, e.КолЗ, e.Количество, e.ЦенаГрн, e.Ss,
            //        e.КолСв, e.Warranty, e.Sozdal, e.Pricemin, e.Crosskurs, e.KolonkaDefault, e.Isaction,
            //        e.BonuslevelId, e.PriceRozn, e.PriceDefault, e.QPart, e.Exps, e.PriceBc, e.SsBc, e.НомерПн
            //    })
            //    .HasName("Расход_НомерПН_price_BC_SS_BC");

            //entity.Property(e => e.BonuslevelId).HasColumnName("bonuslevelID");

            //entity.Property(e => e.Crosskurs)
            //    .HasColumnName("crosskurs")
            //    .HasColumnType("decimal(10, 6)");

            //entity.Property(e => e.Exps)
            //    .HasColumnName("exps")
            //    .HasColumnType("money")
            //    .HasComment("Затраты на 1шт в валюте документа");

            //entity.Property(e => e.ExpsBc)
            //    .HasColumnName("exps_BC")
            //    .HasColumnType("money")
            //    .HasComment("Затраты на 1шт в валюте упр. учета");

            //entity.Property(e => e.Isaction).HasColumnName("isaction");

            //entity.Property(e => e.KolonkaDefault).HasColumnName("kolonka_default");

            //entity.Property(e => e.PriceBc)
            //    .HasColumnName("price_BC")
            //    .HasColumnType("money")
            //    .HasComment("Цена в валюте упр. учета");

            //entity.Property(e => e.PriceDefault)
            //    .HasColumnName("price_default")
            //    .HasColumnType("money");

            //entity.Property(e => e.PriceRozn)
            //    .HasColumnName("price_rozn")
            //    .HasColumnType("money");

            //entity.Property(e => e.Pricemin)
            //    .HasColumnName("pricemin")
            //    .HasColumnType("money");

            //entity.Property(e => e.QPart).HasColumnName("q_part");

            //entity.Property(e => e.Sozdal)
            //    .HasColumnName("sozdal")
            //    .HasMaxLength(10)
            //    .IsUnicode(false)
            //    .IsFixedLength();

            //entity.Property(e => e.Ss)
            //    .HasColumnName("SS")
            //    .HasColumnType("money")
            //    .HasComment("Себестоимость на момент продажи в валюте товара");

            //entity.Property(e => e.SsBc)
            //    .HasColumnName("SS_BC")
            //    .HasColumnType("money")
            //    .HasComment("Себестоимость в валюте упр. учета");

            //entity.Property(e => e.КолСв).HasColumnName("КолСВ");

            //entity.Property(e => e.Марка)
            //    .HasColumnName("марка")
            //    .HasMaxLength(300);

            #endregion
        }
    }
}
