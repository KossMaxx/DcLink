using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.ProductTypes.AddProductTypeMap
{
    public class AddProductTypeMapCommand : BaseMapCommand
    {
        public AddProductTypeMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {}
    }
}
