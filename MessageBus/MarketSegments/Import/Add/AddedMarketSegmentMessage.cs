using System;

namespace MessageBus.MarketSegments.Import.Add
{
    public class AddedMarketSegmentMessage : BaseMessage
    {
        public Guid Value { get; set; }
    }
}
