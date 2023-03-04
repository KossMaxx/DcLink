using System;
using System.Collections.Generic;
using System.Text;
using LegacySql.Domain.Countries;
using LegacySql.Domain.Pictures;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Products
{
    public class Product : Mapped
    {
        public IdMap Code { get; set; }
        public string Brand { get; set; }
        public string WorkName { get; set; }
        public IdMap ProductTypeId { get; set; }
        public IdMap SubtypeId { get; set; }
        public StringIdMap ProductCategoryId { get; set; }
        public IdMap ManufactureId { get; set; }
        public string VendorCode { get; set; }
        public string NomenclatureBarcode { get; set; }
        public string NameForPrinting { get; set; }
        public string ProductCountry { get; set; }
        public string BrandCountry { get; set; }
        public double? Weight { get; set; }
        public double? Volume { get; set; }
        public float? PackageQuantity { get; set; }
        public byte? Guarantee { get; set; }
        public byte? GuaranteeIn { get; set; }
        public string Unit { get; set; }
        public decimal Vat { get; set; }
        public bool? IsImported { get; set; }
        public string NomenclatureCode { get; set; }
        public bool? IsProductIssued { get; set; }
        public string ContentUser { get; set; }
        public int InStock { get; set; }
        public int InReserve { get; set; }
        public int Pending { get; set; }
        public string VideoUrl { get; set; }
        public bool? IsDistribution { get; set; }
        public bool? ScanMonitoring { get; set; }
        public bool? ScanHotline { get; set; }
        public bool? Game { get; set; }
        public bool? ManualRrp { get; set; }
        public bool? NotInvolvedInPricing { get; set; }
        public byte Monitoring { get; set; }
        public bool? Price { get; set; }
        public bool? Markdown { get; set; }
        public string ManufactureSiteLink { get; set; }
        public IEnumerable<ProductCategoryParameter> Parameters { get; set; }
        public IEnumerable<ProductPicture> Pictures { get; set; }
        public IEnumerable<ProductVideo> Video { get; set; }
        public IdMap ManagerId { get; set; }
        public DateTime? ChangedAt { get; set; }
        public string DescriptionRu { get; set; }
        public string DescriptionUa { get; set; }
        public byte? CurrencyId { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }
        public int? NonCashProductId { get; set; }
        public bool IsProduction { get; set; }

        public Product(bool hasMap) : base(hasMap) { }

        public MappingInfo IsMappingsFull()
        {
            var isMappingsFull = true;
            var why = new StringBuilder();
            if (ProductTypeId != null && !ProductTypeId.ExternalId.HasValue)
            {
                why.Append($"Поле: ProductTypeId. Id: {ProductTypeId?.InnerId}\n");
                isMappingsFull = false;
            }

            if (ManufactureId != null && !ManufactureId.ExternalId.HasValue)
            {
                why.Append($"Поле: ManufactureId. Id: {ManufactureId.InnerId}\n");
                isMappingsFull = false;
            }
            if (ProductCategoryId != null && !ProductCategoryId.ExternalId.HasValue)
            {
                why.Append($"Поле: ProductCategoryId. Id: {ProductCategoryId.InnerId}\n");
                isMappingsFull = false;
            }

            if (SubtypeId != null && !SubtypeId.ExternalId.HasValue)
            {
                why.Append($"Поле: SubtypeId. Id: {SubtypeId.InnerId}\n");
                isMappingsFull = false;
            }

            return new MappingInfo
            {
                IsMappingFull = isMappingsFull,
                Why = why.ToString()
            };
        }

        public bool IsNew()
        {
            return !HasMap;
        }

        public bool IsChanged()
        {
            return Code?.ExternalId != null;
        }
    }
}