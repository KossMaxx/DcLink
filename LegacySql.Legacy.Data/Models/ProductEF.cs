using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("lSQL_v_Товары")]
    public class ProductEF
    {
        public int Code { get; set; }//КодТовара
        public string Brand { get; set; }//Марка
        public string WorkName { get; set; }//Позиция
        public int? ProductTypeId { get; set; }//КодТипа
        public string Subtype { get; set; }//подтип
        public string ProductCategory { get; set; }//klas
        public string Manufacture { get; set; }//manufacture
        public string VendorCode { get; set; }//artikul
        public string NomenclatureBarcode { get; set; }//EAN
        public string NameForPrinting { get; set; }//beznal
        public int ProductCountryId { get; set; }//countryOfOrigin_ID
        public int BrandCountryId { get; set; }//countryOfRegistration_ID
        public double? Weight { get; set; }//вес
        public double? Volume { get; set; }//обьем
        public float? PackageQuantity { get; set; }//kolpak
        public byte? Guarantee { get; set; }//war
        public byte? GuaranteeIn { get; set; }//warin
        public string Unit { get; set; }//изм
        public decimal Vat { get; set; }//VAT
        public bool? IsImported { get; set; }//о_импрот
        public string NomenclatureCode { get; set; }//KodZED
        public bool? IsProductIssued { get; set; }//contentOK
        public string ContentUser { get; set; }//contentUser
        public int InStock { get; set; }//нал_ф
        public int InReserve { get; set; }//нал_резерв
        public int Pending { get; set; }//нал_ожид
        public string VideoUrl { get; set; }
        public bool? IsDistribution { get; set; }//is_distribution
        public bool? ScanMonitoring { get; set; } //scan_monitoring
        public bool? ScanHotline { get; set; }//scan_hotline
        public bool? Game { get; set; }//game
        public bool? ManualRrp { get; set; }
        public bool? NotInvolvedInPricing { get; set; }
        public byte Monitoring { get; set; }
        public bool? Price { get; set; }
        public bool? Markdown { get; set; }//уценка
        public DateTime? LastChangeDate { get; set; }
        public int? NonCashProductId { get; set; }
        public byte? Currency { get; set; }//ВалютаТовара
        public virtual IEnumerable<ProductPictureEF> Pictures { get; set; }
        public virtual IEnumerable<ProductVideoEF> Video { get; set; }
        public string ManagerNickName { get; set; } //ProductManager

        public decimal? Price0 { get; set; }//Цена0
        public decimal? Price1 { get; set; }//Цена1
        public decimal? DistributorPrice { get; set; }//Цена2
        public decimal? RRPPrice { get; set; } //Цена3
        public decimal? SpecialPrice { get; set; } //Цена4
        public decimal? MinPrice { get; set; }//Цена5
        public decimal? InternetPrice { get; set; }//ЦенаИ
        public decimal? FirstCost { get; set; }//SS
        public DateTime? DataLastPriceChange { get; set; }
        public decimal? PriceMinBnuah { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }
        
        public decimal? GetPrice(short columnId)
        {
            return columnId == 1 ? Price1
                : columnId == 6 ? Price0
                : columnId == 2 ? DistributorPrice
                : columnId == 3 ? RRPPrice
                : columnId == 9 ? SpecialPrice
                : columnId == 5 ? MinPrice
                : columnId == 11 ? InternetPrice
                : columnId == 8 ? FirstCost
                : 0;
        }

        public ProductEF GetCopy()
        {
            return new ProductEF
            {
                Code = Code,
                Brand = Brand,
                WorkName = WorkName,
                ProductTypeId = ProductTypeId,
                Subtype = Subtype,
                ProductCategory = ProductCategory,
                Manufacture = Manufacture,
                VendorCode = VendorCode,
                NomenclatureBarcode = NomenclatureBarcode,
                NameForPrinting = NameForPrinting,
                ProductCountryId = ProductCountryId,
                BrandCountryId = BrandCountryId,
                Weight = Weight,
                Volume = Volume,
                PackageQuantity = PackageQuantity,
                Guarantee = Guarantee,
                GuaranteeIn = GuaranteeIn,
                Unit = Unit,
                Vat = Vat,
                IsImported = IsImported,
                NomenclatureCode = NomenclatureCode,
                IsProductIssued = IsProductIssued,
                ContentUser = ContentUser,
                InStock = InStock,
                InReserve = InReserve,
                Pending = Pending,
                VideoUrl = VideoUrl,
                IsDistribution = IsDistribution,
                ScanMonitoring = ScanMonitoring,
                ScanHotline = ScanHotline,
                Game = Game,
                ManualRrp = ManualRrp,
                NotInvolvedInPricing = NotInvolvedInPricing,
                Monitoring = Monitoring,
                Price = Price,
                Markdown = Markdown,
                LastChangeDate = LastChangeDate,
                NonCashProductId = NonCashProductId,
                Currency = Currency,
                Pictures = Pictures,
                Video = Video,
                ManagerNickName = ManagerNickName,
                Price0 = Price0,
                Price1 = Price1,
                DistributorPrice = DistributorPrice,
                RRPPrice = RRPPrice,
                SpecialPrice = SpecialPrice,
                MinPrice = MinPrice,
                InternetPrice = InternetPrice,
                FirstCost = FirstCost,
                DataLastPriceChange = DataLastPriceChange,
                PriceMinBnuah = PriceMinBnuah,
                Height = Height,
                Width = Width,
                Depth = Depth
            };
        }

        public void SetPrice(int columnId, decimal? value)
        {
            switch (columnId)
            {
                case 1: Price1 = value;
                    break;
                case 6:
                    Price0 = value;
                    break;
                case 2:
                    DistributorPrice = value;
                    break;
                case 3:
                    RRPPrice = value;
                    break;
                case 9:
                    SpecialPrice = value;
                    break;
                case 5:
                    MinPrice = value;
                    break;
                case 11:
                    InternetPrice = value;
                    break;
                case 8:
                    FirstCost = value;
                    break;
                default:
                {
                    throw new ArgumentException($"Номер колонки {columnId} недопустим");
                }
            }
        }
    }
}
