using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Deliveries.Export
{
    public class DeliveryDto
    {
        public int Code { get; set; }
        public DateTime? CreateDate { get; set; }
        public string DeparturePlanDate { get; set; }
        public string CreatorUsername { get; set; }
        public string TaskDescription { get; set; }
        public string Address { get; set; }
        public string CarrierTypeName { get; set; }
        public string CargoInvoice { get; set; }
        public int StatusId { get; set; }
        public DeliveryCategoryDto Category { get; set; }
        public Guid? ReceivedEmployeeId { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? PerformedDate { get; set; }
        public bool IsFinished { get; set; }
        public IEnumerable<string> Stickers { get; set; }
        public IEnumerable<DeliveryWarehouseDto> Warehouses { get; set; }
    }
}
