using System;
using System.Collections.Generic;

namespace MessageBus.Purchases.Export
{
    public class PurchaseDto
    {
        public Guid? Id { get; set; }
        public int PurchaseSqlId { get; set; }
        public DateTime Date { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? WarehouseId { get; set; }
        public string Comments { get; set; }
        public string SupplierDocument { get; set; }
        public bool IsExecuted { get; set; }
        public bool IsPaid { get; set; }
        public string EmployeeUsername { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string RecipientOKPO { get; set; }
        public int? FirmSqlId { get; set; }
        public bool IsActual { get; set; }
        public IEnumerable<PurchaseItemDto> Items { get; set; }
        public IEnumerable<int> BillNumbers { get; set; }
    }
}
