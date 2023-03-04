using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.Cashboxes.AddCashboxApplicationPaymentMap
{
    public class AddCashboxApplicationPaymentMapCommand : BaseMapCommand
    {
        public AddCashboxApplicationPaymentMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
