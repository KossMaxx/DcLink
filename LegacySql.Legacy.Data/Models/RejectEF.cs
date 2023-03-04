using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("brak")]
    public class RejectEF
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime Date { get; set; }
        public string SerialNumber { get; set; }
        public string ClientTitle { get; set; }
        public int? ClientId { get; set; }
        public byte StatusForClient { get; set; }
        public int WarehouseId { get; set; }
        public string ResponsibleForStatus { get; set; }
        public byte RepairType { get; set; }
        public string DefectDescription { get; set; }
        public string KitDescription { get; set; }
        public string ProductStatusDescription { get; set; }
        public string Notes { get; set; }
        public string ProductStatus { get; set; }
        public int? ClientOrderId { get; set; }
        public virtual ClientOrderEF ClientOrder { get; set; }
        public DateTime? ClientOrderDate { get; set; }
        public DateTime? ReceiptDocumentDate { get; set; }
        public string ReceiptDocumentId { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierTitle { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string ProductMark { get; set; }
        public int ProductId { get; set; }
        public virtual ProductGeneralEF Product { get; set; }
        public int? ProductTypeId { get; set; }
        public decimal? PurchaseCurrencyPrice { get; set; }
        public short? OutgoingWarranty { get; set; }
        public DateTime? DepartureDate  { get; set; }
        public DateTime? ChangedAt { get; set; }
        public byte StatusForService { get; set; }
        public int? ProductRefundId { get; set; }
        public virtual ProductRefundEF ProductRefund { get; set; }
        public string SupplierDescription { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string SupplierProductMark { get; set; }
        public int? SupplierProductId { get; set; }
        public virtual ProductGeneralEF SupplierProduct { get; set; }
        public string SupplierSerialNumber { get; set; }


        public int Amount => 1;
        public DateTime? BuyDocDate  { get; set; }
        public DateTime? SellDocDate  { get; set; }
    }
}
