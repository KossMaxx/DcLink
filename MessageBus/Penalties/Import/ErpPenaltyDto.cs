using System;

namespace MessageBus.Penalties.Import
{
    public class ErpPenaltyDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime Date { get; set; }
        public decimal Sum { get; set; }
    }
}
