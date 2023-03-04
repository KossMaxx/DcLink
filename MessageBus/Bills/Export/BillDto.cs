using System;
using System.Collections.Generic;

namespace MessageBus.Bills.Export
{
    public class BillDto
    {
        public int SqlId { get; set; }
        public DateTime? Date { get; set; }
        public Guid? ClientId { get; set; }
        public string Comments { get; set; }
        public string SellerOkpo { get; set; }
        public DateTime ValidToDate { get; set; }
        public string FirmOkpo { get; set; }
        public int? FirmSqlId { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? ManagerId { get; set; }
        public int? Number { get; set; }
        public bool? Issued { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalUah { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<BillItemDto> Items { get; set; }
        public BillDeliveryDto Delivery { get; set; }
    }
}
