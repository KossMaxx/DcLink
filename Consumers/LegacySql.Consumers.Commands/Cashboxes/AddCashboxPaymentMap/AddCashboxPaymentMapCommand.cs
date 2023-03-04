using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.Cashboxes.AddCashboxPaymentMap
{
    public class AddCashboxPaymentMapCommand : BaseMapCommand
    {
        public AddCashboxPaymentMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
