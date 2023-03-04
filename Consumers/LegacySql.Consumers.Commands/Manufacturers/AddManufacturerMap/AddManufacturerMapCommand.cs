using System;
using LegacySql.Commands.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.Manufacturers.AddManufacturerMap
{
    public class AddManufacturerMapCommand : BaseMapCommand
    {
        public AddManufacturerMapCommand(Guid messageId, Guid externalMapId) : base(messageId, externalMapId)
        {
        }
    }
}
