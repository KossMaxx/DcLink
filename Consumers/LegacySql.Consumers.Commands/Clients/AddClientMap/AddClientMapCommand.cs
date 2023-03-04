using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.Clients.AddClientMap
{
    public class AddClientMapCommand : BaseMapCommand
    {
        public AddClientMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {}
    }
}
