using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.PhysicalPersons.AddPhysicalPersonMap
{
    public class AddPhysicalPersonMapCommand : BaseMapCommand
    {
        public AddPhysicalPersonMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        { }
    }
}
