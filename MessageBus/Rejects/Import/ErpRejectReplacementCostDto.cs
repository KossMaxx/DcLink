using System;

namespace MessageBus.Rejects.Import
{
    public class ErpRejectReplacementCostDto
    {
        public Guid RejectId { get; set; }
        public decimal ReplacementCost { get; set; }
    }
}
