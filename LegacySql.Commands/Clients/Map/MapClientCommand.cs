using System;
using MediatR;

namespace LegacySql.Commands.Clients.Map
{
    public class MapClientCommand : IRequest
    {
        public MapClientCommand(int innerId, Guid externalId)
        {
            InnerId = innerId;
            ExternalId = externalId;
        }

        public int InnerId { get; }
        public Guid ExternalId { get; }
    }
}
