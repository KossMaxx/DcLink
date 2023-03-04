using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace LegacySql.Legacy.Data.Models
{
    [Table("Товары")]
    public class ProductGeneralEF
    {
        public int Code { get; set; }
        public string Brand { get; set; }
        public string WorkName { get; set; }
        public int? ProductTypeId { get; set; }
        public string Subtype { get; set; }
        public string ProductCategory { get; set; }
        public string Manufacture { get; set; }
        public string VendorCode { get; set; }
        public string NomenclatureBarcode { get; set; }
        public string NameForPrinting { get; set; }
        public int ProductCountryId { get; set; }
        public int BrandCountryId { get; set; }
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
        public DateTime? LastChangeDate { get; set; }
        public int? NonCashProductId { get; set; }
        public byte? Currency { get; set; }
    }
}
