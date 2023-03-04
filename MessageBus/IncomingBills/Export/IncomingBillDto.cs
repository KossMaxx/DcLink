using System;
using System.Collections.Generic;

namespace MessageBus.IncomingBills.Export
{
    public class IncomingBillDto
    {
        public int Code { get; set; }
        public DateTime? Date { get; set; }
        public string IncominNumber { get; set; }
        public string RecipientOkpo { get; set; }
        public string SupplierOkpo { get; set; }
        public int? SupplierSqlId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? PurchaseId { get; set; }
        public IEnumerable<IncomingBillItemDto> Items { get; set; }
    }
}
