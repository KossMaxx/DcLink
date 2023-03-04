using System;

namespace MessageBus.Bonuses.Import
{
    public class ErpBonusDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public DateTime Date { get; set; }
        public decimal Sum { get; set; }
    }
}
