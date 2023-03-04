using System;

namespace LegacySql.Legacy.Data.Deliveries
{
    public class DeliveryData
    {
        public int Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public string DeparturePlanDate { get; set; }
        public string CreatorUsername { get; set; }
        public string TaskDescription { get; set; }
        public string Address { get; set; }
        public string CarrierTypeName { get; set; }
        public string CargoInvoice { get; set; }
        public int StatusId { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryTitle { get; set; }
        public int ReceivedEmployeeId { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? PerformedDate { get; set; }
        public bool IsFinished { get; set; }
        public DateTime? ChangedAt { get; set; }

    }
}
