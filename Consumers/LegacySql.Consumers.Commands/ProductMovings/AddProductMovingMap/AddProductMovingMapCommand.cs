using LegacySql.Commands.Shared;
using System;

namespace LegacySql.Consumers.Commands.ProductMovings.AddProductMovingMap
{
    public class AddProductMovingMapCommand : BaseMapCommand
    {
        public AddProductMovingMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
