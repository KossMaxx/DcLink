using System;

namespace LegacySql.Legacy.Data.Products
{
    internal class ProductData
    {
        public int Product_row_id { get; set; }
        public int Product_Code { get; set; }
        public string Product_Brand { get; set; }
        public string Product_WorkName { get; set; }
        public int? Product_ProductTypeId { get; set; }
        public string Product_Subtype { get; set; }
        public string Product_ProductCategory { get; set; }
        public string Product_Manufacture { get; set; }
        public int Product_ManufactureId { get; set; }
        public string Product_VendorCode { get; set; }
        public string Product_NomenclatureBarcode { get; set; }
        public string Product_NameForPrinting { get; set; }
        public int Product_ProductCountryId { get; set; }
        public int Product_BrandCountryId { get; set; }
        public double? Product_Weight { get; set; }
        public double? Product_Volume { get; set; }
        public float? Product_PackageQuantity { get; set; }
        public byte? Product_Guarantee { get; set; }
        public byte? Product_GuaranteeIn { get; set; }
        public string Product_Unit { get; set; }
        public decimal Product_Vat { get; set; }
        public bool? Product_IsImported { get; set; }
        public string Product_NomenclatureCode { get; set; }
        public bool? Product_IsProductIssued { get; set; }
        public string Product_ContentUser { get; set; }
        public int Product_InStock { get; set; }
        public int Product_InReserve { get; set; }
        public int Product_Pending { get; set; }
        public string Product_VideoUrl { get; set; }
        public bool? Product_IsDistribution { get; set; }
        public bool? Product_ScanMonitoring { get; set; }
        public bool? Product_ScanHotline { get; set; }
        public bool? Product_Game { get; set; }
        public bool? Product_ManualRrp { get; set; }
        public bool? Product_NotInvolvedInPricing { get; set; }
        public byte Product_Monitoring { get; set; }
        public bool? Product_Price { get; set; }
        public bool? Product_Markdown { get; set; }
        public DateTime? Product_LastChangeDate { get; set; }
        public int? Product_NonCashProductId { get; set; }
        public byte? Product_Currency { get; set; }
        public string Product_ManagerNickName { get; set; }
        public int Product_ManagerId { get; set; }
        public decimal? Product_Price0 { get; set; }
        public decimal? Product_Price1 { get; set; }
        public decimal? Product_DistributorPrice { get; set; }
        public decimal? Product_RRPPrice { get; set; }
        public decimal? Product_SpecialPrice { get; set; }
        public decimal? Product_MinPrice { get; set; }
        public decimal? Product_InternetPrice { get; set; }
        public decimal? Product_FirstCost { get; set; }
        public DateTime? Product_DataLastPriceChange { get; set; }
        public decimal? Product_PriceMinBnuah { get; set; }
        public double? Product_Height { get; set; }
        public double? Product_Width { get; set; }
        public double? Product_Depth { get; set; }
        public string Product_ManufactureSiteLink { get; set; }
        public PictureData Picture { get; set; }
        public VideoData Video { get; set; }
        public bool Product_Production { get; set; }
    }
}
