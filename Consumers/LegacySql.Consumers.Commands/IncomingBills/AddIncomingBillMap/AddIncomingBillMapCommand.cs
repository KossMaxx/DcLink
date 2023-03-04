using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.IncomingBills.AddIncomingBillMap
{
    public class AddIncomingBillMapCommand : BaseMapCommand
    {
        public AddIncomingBillMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
