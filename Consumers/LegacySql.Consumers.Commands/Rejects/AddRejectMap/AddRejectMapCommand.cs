using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.Rejects.AddRejectMap
{
    public class AddRejectMapCommand : BaseMapCommand
    {
        public AddRejectMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId) { }
    }
}