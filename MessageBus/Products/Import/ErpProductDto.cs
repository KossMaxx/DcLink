using System;
using System.Collections.Generic;

namespace MessageBus.Products.Import
{
    public class ErpProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid? TypeId { get; set; }
        public Guid? ManagerId { get; set; }
        public Guid? SubtypeId { get; set; }
        public IEnumerable<ErpProductParametersDto> Parameters { get; set; }
        public double? Weight { get; set; }
        public double? Volume { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }
        public string DescriptionRu { get; set; }
        public string DescriptionUa { get; set; }
        public IEnumerable<string> PicturesUrls { get; set; }
        public string VendorCode { get; set; }
        public bool Price { get; set; }
        public Guid? Class { get; set; }
        public bool ManualRrp { get; set; }
        public bool NotInvolvedInPricing { get; set; }
        public byte Monitoring { get; set; }
        public byte? Guarantee { get; set; }
        public byte? GuaranteeIn { get; set; }
        public Guid? ManufactureId { get; set; }
        public string NomenclatureBarcode { get; set; }
        public string NameForPrinting { get; set; }
        public string Unit { get; set; }
        public decimal Vat { get; set; }
        public bool? IsImported { get; set; }
        public string NomenclatureCode { get; set; }
        public bool? IsProductIssued { get; set; }
        public bool? IsDistribution { get; set; }
        public bool? ScanMonitoring { get; set; }
        public bool? ScanHotline { get; set; }
        public bool? Game { get; set; }
        public bool? Markdown { get; set; }
        public string ManufactureSiteLink { get; set; }
        public string ProductCountryIsoCode { get; set; }
        public string BrandCountryIsoCode { get; set; }
        public byte? CurrencyId { get; set; }
    }
}
