using System;

namespace MessageBus.ReconciliationActs.Export
{
    public class ReconciliationActDto
    {
        public decimal? Sum { get; set; }
        public Guid? ClientId { get; set; }
        public bool? IsApproved { get; set; }
    }
}
