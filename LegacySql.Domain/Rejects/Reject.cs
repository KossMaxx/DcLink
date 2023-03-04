using System;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Rejects
{
    public class Reject : Mapped
    {
        public IdMap Id { get; }
        public DateTime? CreatedAt { get; }
        public DateTime Date { get; }
        public string SerialNumber { get; }
        public string ClientTitle { get; }
        public IdMap ClientId { get; }
        public byte StatusForClient { get; }
        public IdMap WarehouseId { get; }
        public string ResponsibleForStatus { get; }
        public byte RepairType { get; }
        public string DefectDescription { get; }
        public string KitDescription { get; }
        public string ProductStatusDescription { get; }
        public string Notes { get; }
        public string ProductStatus { get; }
        public IdMap ClientOrderId { get; }
        public DateTime? ClientOrderDate { get; }
        public DateTime? ReceiptDocumentDate { get; }
        public string ReceiptDocumentId { get; }
        public IdMap SupplierId { get; }
        public string SupplierTitle { get; }
        public decimal? PurchasePrice { get; }
        public string ProductMark { get; }
        public IdMap ProductId { get; }
        public IdMap ProductTypeId { get; }
        public decimal? PurchaseCurrencyPrice { get; }
        public short? OutgoingWarranty { get; }
        public DateTime? DepartureDate { get; }
        public DateTime? ChangedAt { get; }
        public int Amount { get; set; }
        public IdMap ProductRefundId { get; }
        public DateTime? BuyDocDate  { get; }
        public DateTime? SellDocDate  { get; }
        public string SupplierDescription { get; }
        public DateTime? ReturnDate { get; }
        public string SupplierProductMark { get; }
        public IdMap SupplierProductId { get; }
        public string SupplierSerialNumber { get; }
        public byte ReturnType { get; }

        public Reject(
            bool hasMap, 
            IdMap id, 
            DateTime? createdAt, 
            DateTime date, 
            string serialNumber, 
            string clientTitle, 
            IdMap clientId, 
            byte statusForClient, 
            IdMap warehouseId, 
            string responsibleForStatus, 
            byte repairType, 
            string defectDescription, 
            string kitDescription, 
            string productStatusDescription, 
            string notes, 
            string productStatus, 
            IdMap clientOrderId, 
            DateTime? clientOrderDate, 
            DateTime? receiptDocumentDate, 
            string receiptDocumentId, 
            IdMap supplierId, 
            string supplierTitle, 
            decimal? purchasePrice, 
            string productMark, 
            IdMap productId, 
            IdMap productTypeId, 
            decimal? purchaseCurrencyPrice, 
            short? outgoingWarranty, 
            DateTime? departureDate, 
            DateTime? changedAt, 
            int amount, 
            IdMap productRefundId, 
            DateTime? buyDocDate, 
            DateTime? sellDocDate, 
            string supplierDescription, 
            DateTime? returnDate, 
            string supplierProductMark, 
            IdMap supplierProductId, 
            string supplierSerialNumber, 
            byte returnType) : base(hasMap)
        {
            Id = id;
            CreatedAt = createdAt;
            Date = date;
            SerialNumber = serialNumber;
            ClientTitle = clientTitle;
            ClientId = clientId;
            StatusForClient = statusForClient;
            WarehouseId = warehouseId;
            ResponsibleForStatus = responsibleForStatus;
            RepairType = repairType;
            DefectDescription = defectDescription;
            KitDescription = kitDescription;
            ProductStatusDescription = productStatusDescription;
            Notes = notes;
            ProductStatus = productStatus;
            ClientOrderId = clientOrderId;
            ClientOrderDate = clientOrderDate;
            ReceiptDocumentDate = receiptDocumentDate;
            ReceiptDocumentId = receiptDocumentId;
            SupplierId = supplierId;
            SupplierTitle = supplierTitle;
            PurchasePrice = purchasePrice;
            ProductMark = productMark;
            ProductId = productId;
            ProductTypeId = productTypeId;
            PurchaseCurrencyPrice = purchaseCurrencyPrice;
            OutgoingWarranty = outgoingWarranty;
            DepartureDate = departureDate;
            ChangedAt = changedAt;
            Amount = amount;
            ProductRefundId = productRefundId;
            BuyDocDate = buyDocDate;
            SellDocDate = sellDocDate;
            SupplierDescription = supplierDescription;
            ReturnDate = returnDate;
            SupplierProductMark = supplierProductMark;
            SupplierProductId = supplierProductId;
            SupplierSerialNumber = supplierSerialNumber;
            ReturnType = returnType;
        }

        public MappingInfo IsMappingsFull()
        {
            var why = new StringBuilder();

            void CheckExternalId(IdMap idMap, string name)
            {
                if (idMap != null && idMap.ExternalId == null)
                {
                    why.Append($"Поле {name}.Id: {idMap?.InnerId}\n");
                }
            }

            CheckExternalId(ClientId, nameof(ClientId));
            CheckExternalId(WarehouseId, nameof(WarehouseId));
            CheckExternalId(SupplierId, nameof(SupplierId));
            CheckExternalId(ProductId, nameof(ProductId));
            CheckExternalId(ProductTypeId, nameof(ProductTypeId));
            //CheckExternalId(ProductRefundId, nameof(ProductRefundId));

            if (SupplierProductId != null)
            {
                CheckExternalId(SupplierProductId, nameof(SupplierProductId));
            }

            var whyResult = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyResult),
                Why = whyResult,
            };
        }

        public bool IsNew()
        {
            return !HasMap;
        }

        public bool IsChanged()
        {
            return Id?.ExternalId != null;
        }
    }
}