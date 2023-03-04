using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.PartnerProductGroups.AddPartnerProductGroup
{
    public class AddPartnerProductGroupCommand : BaseMapCommand
    {
        public AddPartnerProductGroupCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
