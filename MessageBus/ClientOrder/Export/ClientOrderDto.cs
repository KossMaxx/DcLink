using System;
using System.Collections.Generic;
using LegacySql.Domain.Shared;

namespace MessageBus.ClientOrder.Export
{
    public class ClientOrderDto
    {
        public int Number { get; set; }
        public DateTime? Date { get; set; }
        public Guid? ClientId { get; set; }
        public PaymentTypes PaymentType { get; set; }
        public string Comments { get; set; }
        public string MarketplaceNumber { get; set; }
        public DepartmentDto Source { get; set; }
        public OrderDeliveryDto Delivery { get; set; }
        public bool IsExecuted { get; set; }
        public bool IsPaid { get; set; }
        public Guid? WarehouseId { get; set; }
        public Guid? ManagerId { get; set; }
        public int Quantity { get; set; }
        public double Amount { get; set; }
        public string RecipientOKPO { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? BillNumber { get; set; }
        public int? FirmSqlId { get; set; }
        public IEnumerable<ClientOrderItemDto> Items { get; set; }
    }
}
