using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.Deliveries.AddDeliveryMap
{
    public class AddDeliveryMapCommand : BaseMapCommand
    {
        public AddDeliveryMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
