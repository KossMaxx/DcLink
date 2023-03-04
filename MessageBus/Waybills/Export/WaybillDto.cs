using System;
using System.Collections.Generic;

namespace MessageBus.Waybills.Export
{
    public class WaybillDto
    {
        public int Code { get; set; }
        public DateTime Date { get; set; }
        public string CompanyOkpo { get; set; }
        public string Description { get; set; }
        public Guid WarehouseId { get; set; }
        public IEnumerable<Guid> Rejects { get; set; }
    }
}
