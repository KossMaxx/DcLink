using System;
using System.Collections.Generic;

namespace MessageBus.Products.Export
{
    public class ProductDto
    {
        public int Code { get; set; }
        public string Brand { get; set; }
        public string WorkName { get; set; }
        public Guid? SubtypeId { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public Guid? ManufactureId { get; set; }
        public string VendorCode { get; set; }
        public string NomenclatureBarcode { get; set; }
        public string NameForPrinting { get; set; }
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
        public Guid? ProductTypeId { get; set; }
        public string ProductCountryIsoCode { get; set; }
        public string BrandCountryIsoCode { get; set; }
        public IEnumerable<ProductCategoryParameterDto> Parameters { get; set; }
        public IEnumerable<string> Pictures { get; set; }
        public IEnumerable<string> Video { get; set; }
        public Guid? ManagerId { get; set; }
        public string DescriptionRu { get; set; }
        public string DescriptionUa { get; set; }
        public byte? CurrencyId { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }
        public int? NonCashProductId { get; set; }
        public bool IsProduction { get; set; }
    }
}
