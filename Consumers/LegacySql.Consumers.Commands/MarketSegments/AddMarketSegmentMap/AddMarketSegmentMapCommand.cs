using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.MarketSegments.AddMarketSegmentMap
{
    public class AddMarketSegmentMapCommand : BaseMapCommand
    {
        public AddMarketSegmentMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
