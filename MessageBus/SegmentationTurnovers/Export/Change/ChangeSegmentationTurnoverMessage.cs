using System;

namespace MessageBus.SegmentationTurnovers.Export.Change
{
    public class ChangeSegmentationTurnoverMessage : BaseSagaMessage<SegmentationTurnoverDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
