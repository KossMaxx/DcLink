using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.ProductTypes.AddProductTypeCategoryMap
{
    public class AddProductTypeCategoryMapCommand : BaseMapCommand
    {
        public AddProductTypeCategoryMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        { }
    }
}
