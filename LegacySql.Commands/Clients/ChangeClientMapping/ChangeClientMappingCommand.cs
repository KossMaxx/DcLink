using System;
using MediatR;

namespace LegacySql.Commands.Clients.ChangeClientMapping
{
    public class ChangeClientMappingCommand : IRequest
    {
        public ChangeClientMappingCommand(Guid oldId, Guid newId)
        {
            OldId = oldId;
            NewId = newId;
        }

        public Guid OldId { get; }
        public Guid NewId { get; }
    }
}
