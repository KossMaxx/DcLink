using System;
using MessageBus;

namespace MessageBus.MarketSegments.Export.Change
{
    public class ChangeMarketSegmentMessage : BaseSagaMessage<MarketSegmentDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
