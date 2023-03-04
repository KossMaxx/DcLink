using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.SegmentationTurnovers.AddSegmentationTurnover
{
    public class AddSegmentationTurnoverCommand : BaseMapCommand
    {
        public AddSegmentationTurnoverCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
