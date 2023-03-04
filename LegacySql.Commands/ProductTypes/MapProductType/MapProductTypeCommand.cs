using System;
using MediatR;

namespace LegacySql.Commands.ProductTypes.MapProductType
{
    public class MapProductTypeCommand : IRequest
    {
        public MapProductTypeCommand(int innerId, Guid externalId)
        {
            InnerId = innerId;
            ExternalId = externalId;
        }

        public int InnerId { get; }
        public Guid ExternalId { get; }
    }
}
