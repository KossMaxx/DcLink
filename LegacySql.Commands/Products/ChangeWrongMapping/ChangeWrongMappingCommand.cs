using System;
using MediatR;

namespace LegacySql.Commands.Products.ChangeWrongMapping
{
    public class ChangeWrongMappingCommand : IRequest
    {
        public ChangeWrongMappingCommand(int innerId, Guid externalId)
        {
            InnerId = innerId;
            ExternalId = externalId;
        }

        public int InnerId { get; }
        public Guid ExternalId { get; }
    }
}
