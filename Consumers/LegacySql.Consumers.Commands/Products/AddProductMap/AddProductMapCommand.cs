using System;
using LegacySql.Commands.Shared;

namespace LegacySql.Consumers.Commands.Products.AddProductMap
{
    public class AddProductMapCommand : BaseMapCommand
    {
        public AddProductMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {}
    }
}
