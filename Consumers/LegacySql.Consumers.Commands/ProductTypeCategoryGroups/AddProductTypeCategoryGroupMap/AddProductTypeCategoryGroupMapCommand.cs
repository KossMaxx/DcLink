using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.ProductTypeCategoryGroups.AddProductTypeCategoryGroupMap
{
    public class AddProductTypeCategoryGroupMapCommand : BaseMapCommand
    {
        public AddProductTypeCategoryGroupMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {}
    }
}
