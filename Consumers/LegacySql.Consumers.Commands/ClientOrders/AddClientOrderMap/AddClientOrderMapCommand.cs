using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.ClientOrders.AddClientOrderMap
{
    public class AddClientOrderMapCommand : BaseMapCommand
    {
        public AddClientOrderMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {}
    }
}
