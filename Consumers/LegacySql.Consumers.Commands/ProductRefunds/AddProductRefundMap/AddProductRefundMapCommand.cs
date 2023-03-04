using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.ProductRefunds.AddProductRefundMap
{
    public class AddProductRefundMapCommand : BaseMapCommand
    {
        public AddProductRefundMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {}
    }
}
