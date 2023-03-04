using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.ProductSubtypes.AddProductSubtypeMap
{
    public class AddProductSubtypeMapCommand : BaseMapCommand
    {
        public AddProductSubtypeMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
