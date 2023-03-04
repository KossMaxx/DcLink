using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacySql.Legacy.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductEF>
    {
        public void Configure(EntityTypeBuilder<ProductEF> builder)
        {
            builder.HasKey(e => e.Code)
                .HasName("PK_Товары3");

            builder.HasIndex(e => e.Brand)
                .HasDatabaseName("Марка");

            builder.HasIndex(e => e.WorkName)
                .HasDatabaseName("Позиция");

            builder.HasIndex(e => e.NonCashProductId)
                .HasDatabaseName("Item_beznal_tovID");

            builder.Property(e => e.VendorCode)
                .HasColumnName("artikul")
                .HasMaxLength(48);

            builder.Property(e => e.NameForPrinting)
                .HasColumnName("beznal")
                .HasMaxLength(300);

            builder.Property(e => e.Code).HasColumnName("КодТовара");
            builder.Property(e => e.WorkName).HasMaxLength(400).HasColumnName("Позиция");
            builder.Property(e => e.ProductTypeId).HasColumnName("КодТипа");

            builder.Property(e => e.IsProductIssued).HasColumnName("contentOK");

            builder.Property(e => e.ContentUser)
                .HasColumnName("contentUser")
                .HasMaxLength(20);

            builder.Property(e => e.ProductCountryId).HasColumnName("countryOfOrigin_ID");

            builder.Property(e => e.BrandCountryId).HasColumnName("countryOfRegistration_ID");
            builder.Property(e => e.NomenclatureBarcode)
                .HasColumnName("EAN")
                .HasMaxLength(50);

            builder.Property(e => e.Game).HasColumnName("game");


            builder.Property(e => e.IsDistribution).HasColumnName("is_distribution");

            builder.Property(e => e.ProductCategory)
                .HasColumnName("klas")
                .HasMaxLength(30);

            builder.Property(e => e.NomenclatureCode)
                .HasColumnName("KodZED")
                .HasMaxLength(30);

            builder.Property(e => e.PackageQuantity).HasColumnName("kolpak");

            builder.Property(e => e.ManualRrp).HasColumnName("manual_rrp");

            builder.Property(e => e.Manufacture)
                .HasColumnName("manufacture")
                .HasMaxLength(20);
            builder.Property(e => e.Price).HasColumnName("PRICE");

            builder.Property(e => e.NotInvolvedInPricing).HasColumnName("pricealgoritm_ignore");
            builder.Property(e => e.Monitoring).HasColumnName("RRP");
            builder.Property(e => e.ScanHotline).HasColumnName("scan_hotline");

            builder.Property(e => e.ScanMonitoring).HasColumnName("scan_monitoring");
            builder.Property(e => e.Vat)
                .HasColumnName("VAT")
                .HasColumnType("money");

            builder.Property(e => e.VideoUrl)
                .HasColumnName("videoURL")
                .HasMaxLength(100);

            builder.Property(e => e.Guarantee).HasColumnName("war");

            builder.Property(e => e.GuaranteeIn).HasColumnName("warin");

            builder.Property(e => e.Weight).HasColumnName("вес");

            builder.Property(e => e.Unit)
                .HasColumnName("изм")
                .HasMaxLength(8);

            builder.Property(e => e.Brand).HasMaxLength(300).HasColumnName("Марка");

            builder.Property(e => e.Pending).HasColumnName("нал_ожид");

            builder.Property(e => e.InReserve).HasColumnName("нал_резерв");

            builder.Property(e => e.InStock).HasColumnName("нал_ф");

            builder.Property(e => e.IsImported).HasColumnName("о_импрот");

            builder.Property(e => e.Volume).HasColumnName("обьем");

            builder.Property(e => e.Subtype)
                .HasColumnName("подтип")
                .HasMaxLength(40);

            builder.Property(e => e.Markdown).HasColumnName("уценка");

            builder.Property(e => e.LastChangeDate)
                    .HasColumnName("DataLastChange")
                    .HasColumnType("datetime");

            builder.Property(e => e.NonCashProductId).HasColumnName("beznal_tovID");

            builder.Property(e => e.Currency).HasColumnName("ВалютаТовара").HasColumnType("tinyint");

            builder.HasMany(e => e.Pictures).WithOne(e => e.Product).HasForeignKey(e => e.ProductId);

            builder.Property(e => e.ManagerNickName).HasColumnName("ProductManager");

            builder.Property(e => e.FirstCost)
                    .HasColumnName("SS")
                    .HasColumnType("money");

            builder.Property(e => e.DataLastPriceChange).HasColumnType("datetime");

            builder.Property(e => e.Price0).HasColumnName("Цена0").HasColumnType("money");

            builder.Property(e => e.Price1).HasColumnName("Цена1").HasColumnType("money");

            builder.Property(e => e.DistributorPrice).HasColumnName("Цена2").HasColumnType("money");

            builder.Property(e => e.RRPPrice).HasColumnName("Цена3").HasColumnType("money");

            builder.Property(e => e.SpecialPrice).HasColumnName("Цена4").HasColumnType("money");

            builder.Property(e => e.MinPrice).HasColumnName("Цена5").HasColumnType("money");

            builder.Property(e => e.InternetPrice).HasColumnName("ЦенаИ").HasColumnType("money");

            builder.Property(e => e.PriceMinBnuah)
                .HasColumnName("PriceMinBNuah")
                .HasColumnType("money");
            
            builder.Property(e => e.Height).HasColumnName("height");
            builder.Property(e => e.Width).HasColumnName("width");
            builder.Property(e => e.Depth).HasColumnName("depth");
            
            
            #region AutoGeneretedConfiguration

            //builder.HasIndex(e => new { e.Price, e.Markdown })
            //    .HasName("<PRICEуценка>");

            //builder.HasIndex(e => new { e.Code, e.NomenclatureBarcode })
            //    .HasName("<EAN_tovar>");

            //builder.HasIndex(e => new { e.Code, e.Brand, e.VendorCode, e.TypeOfNomenclature })
            //    .HasName("КодТипа");

            //builder.HasIndex(e => new { e.Code, e.TypeOfNomenclature, e.Цена0, e.Markdown, e.Price })
            //    .HasName("PRICE_INCLUDE_КодТовара_КодТипа_Цена0_уценка");

            //builder.HasIndex(e => new { e.Code, e.InStock, e.НалКомпы, e.SuplId, e.Markdown, e.Price })
            //    .HasName("Tovary_PRICE");

            //builder.HasIndex(e => new { e.Code, e.Цена1, e.Цена3, e.ЦенаИ, e.PriceUah, e.ВалютаТовара })
            //    .HasName("Tovar_ValutaToivara");

            //builder.HasIndex(e => new { e.Code, e.TypeOfNomenclature, e.InStock, e.Pending, e.DataLastSale, e.Markdown, e.Price })
            //    .HasName("КодТовара_КодТипа_нал_ф_нал_ожид_dataLastSale_уценка");

            //builder.HasIndex(e => new { e.Code, e.TypeOfNomenclature, e.InStock, e.Pending, e.DataLastSale, e.Markdown, e.BeznalTovId, e.Price })
            //    .HasName("КодТовара-КодТипа-нал_ф-нал_ожид-dataLastSale-уценка-beznal_tovID");

            //builder.HasIndex(e => new { e.Code, e.InStock, e.НалКомпы, e.Pending, e.InReserve, e.НалРезервОжид, e.Zakaz, e.TypeOfNomenclature, e.Price })
            //    .HasName("КодТипа_PRICE");

            //builder.HasIndex(e => new { e.Code, e.InStock, e.НалКомпы,e.Guarantee, e.InReserve, e.НалРезервОжид, e.ВалютаТовара, e.Markdown, e.Manufacture, e.Subtype, e.VendorCode, e.Pending, e.TypeOfNomenclature, e.Price })
            //    .HasName("КодТипа, Price");

            //builder.HasIndex(e => new { e.Code, e.Brand, e.TypeOfNomenclature, e.Цена0, e.InStock, e.Pending, e.InReserve, e.Pricewatch, e.НалРезервОжид, e.Zakaz, e.Monitoring, e.НалКомпы, e.Subtype, e.VendorCode, e.ВалютаТовара })
            //    .HasName("Товары_валюта_товара");

            //builder.HasIndex(e => new { e.Code, e.Позиция, e.TypeOfNomenclature, e.Цена0, e.InStock, e.НалРезервОжид, e.Zakaz, e.ВалютаТовара, e.НалКомпы, e.Manufacture, e.Subtype, e.VendorCode, e.Pending, e.InReserve, e.Price })
            //    .HasName("dbo_Goods_nalf");

            //builder.HasIndex(e => new { e.Code, e.Позиция, e.TypeOfNomenclature, e.InStock, e.НалКомпы, e.Manufacture, e.Subtype, e.VendorCode, e.Pending, e.InReserve, e.ЦенаИ, e.NameForPrinting, e.НалРезервОжид, e.Zakaz, e.ВалютаТовара, e.BeznalTovId, e.Price })
            //    .HasName("index_opt_bn_search");

            //builder.HasIndex(e => new { e.Code, e.Позиция, e.InStock, e.НалКомпы, e.Manufacture, e.Subtype, e.VendorCode, e.Pending, e.InReserve, e.ЦенаИ, e.NameForPrinting, e.НалРезервОжид, e.Zakaz, e.ВалютаТовара, e.BeznalTovId, e.TypeOfNomenclature, e.Price })
            //    .HasName("index_opt_bn_search_price");

            //builder.HasIndex(e => new { e.DataLastSale, e.Pricewatch, e.Zakaz, e.PriceUah, e.ВалютаТовара, e.Subtype, e.VendorCode, e.PackageQuantity, e.Pending, e.InReserve, e.ЦенаИ, e.Code, e.Brand, e.InStock, e.НалКомпы, e.TypeOfNomenclature, e.Price, e.Manufacture })
            //    .HasName("IX_Товары_КодТипа_PRICE_manufacture");

            //builder.HasIndex(e => new { e.Code, e.TypeOfNomenclature, e.Цена0, e.Цена1, e.ВалютаТовара, e.ManualRrp, e.Manufacture, e.Pending, e.ЦенаИ, e.ProductCategory, e.Zakaz, e.PriceUah, e.Цена3, e.Цена4, e.Цена5, e.InStock, e.НалКомпы, e.Ss, e.ProductManager, e.NotInvolvedInPricing })
            //    .HasName("Tovary_ProductManager_pricealgoritm_ignore");

            //builder.HasIndex(e => new { e.Code, e.TypeOfNomenclature, e.Цена0, e.Цена1, e.Цена3, e.ВалютаТовара, e.ManualRrp, e.ProductManager, e.Pending, e.ЦенаИ, e.ProductCategory, e.Zakaz, e.PriceUah, e.Цена4, e.Цена5, e.InStock, e.НалКомпы, e.Ss, e.Manufacture, e.NotInvolvedInPricing })
            //    .HasName("<Товары_pricealgoritm_ignore>");

            //builder.HasIndex(e => new { e.Code, e.Цена3, e.Ss,e.Guarantee, e.PriceminBnfob, e.BeznalTovId, e.NomenclatureBarcode, e.PriceUah, e.ВалютаТовара, e.Markdown, e.PriceMinBnuah, e.Monitoring, e.Manufacture, e.Subtype, e.VendorCode, e.ЦенаИ, e.NameForPrinting, e.NomenclatureCode, e.TypeOfNomenclature, e.Price })
            //    .HasName("Tovary_KodTipa_Price");

            //builder.HasIndex(e => new { e.Code, e.Brand, e.Позиция, e.Цена0, e.Monitoring, e.ЦенаИ, e.НалРезервОжид, e.Zakaz, e.PriceUah, e.ВалютаТовара, e.Markdown, e.Ss, e.War, e.Manufacture, e.Subtype, e.VendorCode, e.Pending, e.Цена1, e.Цена3, e.Цена4, e.Цена5, e.InStock, e.НалКомпы, e.TypeOfNomenclature, e.Price })
            //    .HasName("Tovary_KodTipa_Price_2");

            //builder.Property(e => e.AbcProfit)
            //    .HasColumnName("ABC_profit")
            //    .HasMaxLength(1)
            //    .IsUnicode(false)
            //    .IsFixedLength();

            //builder.Property(e => e.AbcQty)
            //    .HasColumnName("ABC_qty")
            //    .HasMaxLength(1)
            //    .IsUnicode(false)
            //    .IsFixedLength();

            //builder.Property(e => e.AbcSum)
            //    .HasColumnName("ABC_sum")
            //    .HasMaxLength(1)
            //    .IsUnicode(false)
            //    .IsFixedLength();

            //builder.Property(e => e.Action).HasColumnName("action");

            //    builder.Property(e => e.Bn).HasColumnName("bn");

            //    builder.Property(e => e.BnCalc).HasColumnName("bnCalc");

            //    builder.Property(e => e.Bnproduct).HasColumnName("bnproduct");

            //    builder.Property(e => e.BonusLevel).HasColumnName("bonusLevel");

            //    builder.Property(e => e.ContentTime).HasColumnType("datetime");

            //    builder.Property(e => e.DataLastChange).HasColumnType("datetime");

            //    builder.Property(e => e.DataLastChange1C).HasColumnType("datetime");

            //    builder.Property(e => e.DataLastChangeDescription).HasColumnType("datetime");

            //    builder.Property(e => e.DataLastSale)
            //        .HasColumnName("dataLastSale")
            //        .HasColumnType("smalldatetime");

            //    builder.Property(e => e.DataLastSschange)
            //        .HasColumnName("DataLastSSChange")
            //        .HasColumnType("datetime");

            //    builder.Property(e => e.Ddp).HasColumnName("DDP");

            //    builder.Property(e => e.Description2).HasColumnType("ntext");

            //    builder.Property(e => e.Errdescr)
            //        .HasColumnName("ERRdescr")
            //        .HasColumnType("ntext");
            //    builder.Property(e => e.Gtdid).HasColumnName("GTDid");

            //    builder.Property(e => e.Inet)
            //        .HasColumnName("inet")
            //        .HasMaxLength(255);

            //    builder.Property(e => e.Insurance).HasColumnName("insurance");
            //    builder.Property(e => e.Lasttime)
            //        .HasColumnName("lasttime")
            //        .HasColumnType("datetime");

            //    builder.Property(e => e.MinRozn).HasColumnName("minRozn");

            //    builder.Property(e => e.NalWClosed).HasColumnName("nal_w_closed");

            //    builder.Property(e => e.NeedPack).HasColumnName("need_pack");

            //    builder.Property(e => e.NoPenalty).HasColumnName("no_Penalty");

            //    builder.Property(e => e.PartAlgoritmType).HasColumnName("part_algoritm_type");

            //    builder.Property(e => e.Place)
            //        .HasColumnName("place")
            //        .HasMaxLength(30);

            //    builder.Property(e => e.PriceAlgoritmsQ).HasColumnName("priceAlgoritmsQ");

            //    builder.Property(e => e.PriceCobra).HasColumnName("priceCobra");

            //    builder.Property(e => e.PriceUah)
            //        .HasColumnName("priceUAH")
            //        .HasColumnType("money");

            //    builder.Property(e => e.PriceminBnfob)
            //        .HasColumnName("priceminBNfob")
            //        .HasColumnType("money");

            //    builder.Property(e => e.PriceminBnfobNocheckAvail).HasColumnName("priceminBNfob_nocheck_avail");

            //    builder.Property(e => e.Priceoffer).HasColumnName("priceoffer");

            //    builder.Property(e => e.Pricewatch).HasColumnName("pricewatch");

            //    builder.Property(e => e.ProductId).HasColumnName("product_id");

            //    builder.Property(e => e.ProductManager).HasMaxLength(20);

            //    builder.Property(e => e.QtyRozetka).HasColumnName("qty_rozetka");

            //    builder.Property(e => e.Rebate).HasColumnName("rebate");

            //    builder.Property(e => e.RoznN).HasColumnName("roznN");

            //    builder.Property(e => e.Sbonus)
            //        .HasColumnName("sbonus")
            //        .HasColumnType("money");

            //    builder.Property(e => e.SertificatId).HasColumnName("sertificatID");

            //    builder.Property(e => e.Service).HasColumnName("service");

            //    builder.Property(e => e.Skl).HasColumnName("skl");

            //    builder.Property(e => e.Sn).HasColumnName("sn");

            //    builder.Property(e => e.Sozdal)
            //        .HasColumnName("sozdal")
            //        .HasMaxLength(50);

            //    builder.Property(e => e.SpriceBd)
            //        .HasColumnName("spriceBD")
            //        .HasColumnType("date");

            //    builder.Property(e => e.SpriceEd)
            //        .HasColumnName("spriceED")
            //        .HasColumnType("date");

            //    builder.Property(e => e.SpriceMoq).HasColumnName("spriceMOQ");

            //    builder.Property(e => e.Spriceopt)
            //        .HasColumnName("spriceopt")
            //        .HasColumnType("money");

            //    

            //    builder.Property(e => e.Ssd1).HasColumnName("SSd1");

            //    builder.Property(e => e.Stovar1).HasColumnName("stovar1");

            //    builder.Property(e => e.Stovar2).HasColumnName("stovar2");

            //    builder.Property(e => e.SuplId).HasColumnName("suplID");
            //    builder.Property(e => e.VendorId).HasColumnName("vendor_id");
            //    builder.Property(e => e.VisaAction).HasColumnName("visaAction");

            //    builder.Property(e => e.Zakaz).HasColumnName("zakaz");
            //    builder.Property(e => e.Изменение)
            //        .HasColumnName("изменение")
            //        .HasMaxLength(10);
            //builder.Property(e => e.Нал).HasColumnName("нал");

            //builder.Property(e => e.НалКомпы).HasColumnName("нал_компы");
            //builder.Property(e => e.НалОпт).HasColumnName("нал_опт");
            //builder.Property(e => e.НалРезервОжид).HasColumnName("нал_резерв_ожид");
            //builder.Property(e => e.Прим).HasMaxLength(250);

            //builder.Property(e => e.Продукция).HasColumnName("продукция");

            //builder.Property(e => e.Птк).HasColumnName("ПТК");

            //builder.Property(e => e.СлПрим)
            //    .HasColumnName("сл_прим")
            //    .HasMaxLength(50);

            //builder.Property(e => e.Снят).HasColumnName("снят");

            //builder.Property(e => e.Юзер)
            //    .HasColumnName("юзер")
            //    .HasMaxLength(20);

            #endregion
        }
    }
}
