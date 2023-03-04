using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.Classes.AddClassMap
{
    public class AddClassMapCommand : BaseMapCommand
    {
        public AddClassMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
