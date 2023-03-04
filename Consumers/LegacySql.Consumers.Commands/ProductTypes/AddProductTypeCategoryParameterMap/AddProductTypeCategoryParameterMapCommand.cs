using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.ProductTypes.AddProductTypeCategoryParameterMap
{
    public class AddProductTypeCategoryParameterMapCommand : BaseMapCommand
    {
        public AddProductTypeCategoryParameterMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        { }
    }
}
