using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.Firms.AddFirmMap
{
    public class AddFirmMapCommand : BaseMapCommand
    {
        public AddFirmMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
