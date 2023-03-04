using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.Bills.AddBillMap
{
    public class AddBillMapCommand : BaseMapCommand
    {
        public AddBillMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
