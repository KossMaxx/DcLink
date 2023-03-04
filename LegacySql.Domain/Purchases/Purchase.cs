using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Purchases
{
    public class Purchase : Mapped
    {
        public IdMap Id { get; }
        public int PurchaseSqlId { get; }
        public DateTime Date { get; }
        public bool IsExecuted { get; }
        public string Comments { get; }
        public PurchaseType Type { get; }
        public bool IsActual { get; }
        public decimal? TransportationCost { get; }
        public int? CostType { get; }
        public bool? IsApproved { get; }
        public bool? IsFinancialSideConfirmed { get; }
        public bool? IsProductsArrivedToPort { get; }
        public bool IsCashlessDocumentsProcessNeeded { get; }
        public string SupplierDocument { get; }
        public DateTime? ShippingDate { get; }
        public DateTime? ChangedAt { get; }
        public bool IsPaid { get; set; }
        public IdMap WarehouseId { get; }
        public IdMap ClientId { get; }
        public string EmployeeUsername { get; }
        public DateTime? PaymentDate { get; }
        public string RecipientOKPO { get; }
        public int? FirmSqlId { get; }
        public IEnumerable<PurchaseItem> Items { get; }
        public IEnumerable<int> BillNumbers { get; }

        public Purchase(
            bool hasMap,
            IdMap id,
            int purchaseSqlId,
            DateTime date,
            bool isExecuted,
            string comments,
            PurchaseType type,
            bool isActual,
            decimal? transportationCost,
            int? costType,
            bool? isApproved,
            bool? isFinancialSideConfirmed,
            bool? isProductsArrivedToPort,
            bool isCashlessDocumentsProcessNeeded,
            string supplierDocument,
            DateTime? shippingDate,
            DateTime? changedAt,
            IdMap warehouseId,
            IdMap clientId,
            bool isPaid,
            IEnumerable<PurchaseItem> items,
            string employeeUsername,
            DateTime? paymentDate,
            string recipientOKPO,
            int? firmSqlId,
            IEnumerable<int> billNumbers) : base(hasMap)
        {
            Id = id;
            Date = date;
            IsExecuted = isExecuted;
            Comments = comments;
            Type = type;
            IsActual = isActual;
            TransportationCost = transportationCost;
            CostType = costType;
            IsApproved = isApproved;
            IsFinancialSideConfirmed = isFinancialSideConfirmed;
            IsProductsArrivedToPort = isProductsArrivedToPort;
            IsCashlessDocumentsProcessNeeded = isCashlessDocumentsProcessNeeded;
            SupplierDocument = supplierDocument;
            ShippingDate = shippingDate;
            ChangedAt = changedAt;
            WarehouseId = warehouseId;
            ClientId = clientId;
            IsPaid = isPaid;
            Items = items ?? new List<PurchaseItem>();
            PurchaseSqlId = purchaseSqlId;
            EmployeeUsername = employeeUsername;
            PaymentDate = paymentDate;
            RecipientOKPO = recipientOKPO;
            BillNumbers = billNumbers;
            FirmSqlId = firmSqlId;
        }

        public MappingInfo IsMappingsFull()
        {
            var why = new StringBuilder();
            var isClientMapFull = ClientId?.ExternalId != null;
            if (!isClientMapFull)
            {
                why.Append($"Поле: ClientId., Id: {ClientId?.InnerId}\n");
            }

            var isWareHouseMapFull = WarehouseId?.ExternalId != null;
            if (!isWareHouseMapFull)
            {
                why.Append($"Поле: WarehouseId., Id: {WarehouseId?.InnerId}\n");
            }

            var isItemsMapsFull = Items.All(i => i.ProductId?.ExternalId != null);
            if (!isItemsMapsFull)
            {
                var notFullMapsInfo = Items.Where(i => i.ProductId?.ExternalId == null).Select(i => $"Поле: Item.ProductId., Id: {i.ProductId?.InnerId}\n");
                foreach (var info in notFullMapsInfo)
                {
                    why.Append(info);
                }
            }

            return new MappingInfo
            {
                IsMappingFull = isClientMapFull && isWareHouseMapFull && isItemsMapsFull,
                Why = why.ToString()
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