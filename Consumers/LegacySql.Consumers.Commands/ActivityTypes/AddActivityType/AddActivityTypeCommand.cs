using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.ActivityTypes.AddActivityType
{
    public class AddActivityTypeCommand : BaseMapCommand
    {
        public AddActivityTypeCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
