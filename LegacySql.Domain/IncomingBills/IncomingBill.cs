using LegacySql.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegacySql.Domain.IncomingBills
{
    public class IncomingBill : Mapped
    {
        public IdMap Id { get; }
        public DateTime? Date { get; }
        public string IncominNumber { get; }
        public string RecipientOkpo { get; }
        public string SupplierOkpo { get; }
        public int? SupplierSqlId { get; }
        public IdMap ClientId { get; }
        public DateTime? ChangedAt { get; }
        public IdMap PurchaseId { get; }
        public IEnumerable<IncomingBillItem> Items { get; }

        public IncomingBill(bool hasMap,
            IdMap id,
            DateTime? date,
            string incominNumber,
            string recipientOkpo,
            string supplierOkpo,
            int? supplierSqlId,
            IdMap clientId,
            DateTime? changedAt,
            IdMap purchaseId,
            IEnumerable<IncomingBillItem> items) : base(hasMap)
        {
            Id = id;
            Date = date;
            IncominNumber = incominNumber;
            RecipientOkpo = recipientOkpo;
            SupplierOkpo = supplierOkpo;
            ClientId = clientId;
            Items = items;
            ChangedAt = changedAt;
            PurchaseId = purchaseId;
            SupplierSqlId = supplierSqlId;
        }

        public MappingInfo IsMappingsFull()
        {
            var isClientMappingFull = ClientId?.ExternalId != null;
            var itemsWithoutMappings = Items.Where(i => i.NomenclatureId?.ExternalId == null).ToList();
            var isPurchaseMappingFull = PurchaseId?.ExternalId != null;

            var isMappingFull = isClientMappingFull
                                && itemsWithoutMappings.Count == 0;

            if (isMappingFull)
            {
                return new MappingInfo { IsMappingFull = true, Why = "" };
            }

            var why = new StringBuilder();

            if (!isClientMappingFull)
            {
                why.AppendLine($"Поле: ClientId, Id: {ClientId?.InnerId}");
            }

            if (itemsWithoutMappings.Count > 0)
            {
                foreach (var item in itemsWithoutMappings)
                {
                    why.Append($"Поле: NomenclatureId, Id: {item.NomenclatureId?.InnerId}");
                }
            }

            return new MappingInfo
            {
                IsMappingFull = false,
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
