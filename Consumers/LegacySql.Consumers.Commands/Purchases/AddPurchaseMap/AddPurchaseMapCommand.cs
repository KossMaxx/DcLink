using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.Purchases.AddPurchaseMap
{
    public class AddPurchaseMapCommand : BaseMapCommand
    {
        public AddPurchaseMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {}
    }
}
