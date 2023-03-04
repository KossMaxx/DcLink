using System;
using MediatR;

namespace LegacySql.Commands.Products.SetProductMapping
{
    public class SetProductMappingCommand : IRequest
    {
        public SetProductMappingCommand(int innerId, Guid externalId)
        {
            InnerId = innerId;
            ExternalId = externalId;
        }

        public int InnerId { get; }
        public Guid ExternalId { get; }
    }
}
