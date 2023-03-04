using System;

namespace MessageBus.SegmentationTurnovers.Import.Add
{
    public class AddedSegmentationTurnoverMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
