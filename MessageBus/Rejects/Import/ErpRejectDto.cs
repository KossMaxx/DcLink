using System;

namespace MessageBus.Rejects.Import
{
    public class ErpRejectDto
    {
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string SerialNumber { get; set; }
        public string ClientTitle { get; set; }
        public Guid? ClientId { get; set; }
        public byte StatusForClient { get; set; }
        public Guid WarehouseId { get; set; }
        public string ResponsibleForStatus { get; set; }
        public byte RepairType { get; set; }
        public string DefectDescription { get; set; }
        public string KitDescription { get; set; }
        public string ProductStatusDescription { get; set; }
        public string Notes { get; set; }
        public string ProductStatus { get; set; }
        public Guid? ClientOrderId { get; set; }
        public DateTime? ClientOrderDate { get; set; }
        public DateTime? ReceiptDocumentDate { get; set; }
        public string ReceiptDocumentId { get; set; }
        public Guid? SupplierId { get; set; }
        public string SupplierTitle { get; set; }
        public decimal? PurchasePrice { get; set; }
        public string ProductMark { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ProductTypeId { get; set; }
        public decimal? PurchaseCurrencyPrice { get; set; }
        public short? OutgoingWarranty { get; set; }
        public DateTime? DepartureDate { get; set; }
        public int? ClientOrderSqlId { get; set; }
        public int? PurchaseSqlId { get; set; }
        public Guid? PurchaseId { get; set; }
        public string SupplierDescription { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int ReturnType { get; set; }
        public Guid? SupplierProductId { get; set; }
        public string SupplierSerialNumber { get; set; }
    }
}
