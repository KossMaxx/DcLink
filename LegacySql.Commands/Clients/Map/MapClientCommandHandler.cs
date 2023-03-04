using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Commands.Clients.Map
{
    public class MapClientCommandHandler : IRequestHandler<MapClientCommand>
    {
        private readonly IClientMapRepository _clientMapRepository;

        public MapClientCommandHandler(IClientMapRepository clientMapRepository)
        {
            _clientMapRepository = clientMapRepository;
        }

        public async Task<Unit> Handle(MapClientCommand command, CancellationToken cancellationToken)
        {
            var mapping = await _clientMapRepository.GetByLegacyAsync(command.InnerId);
            if (mapping == null)
            {
                mapping = new ExternalMap(Guid.NewGuid(), command.InnerId, command.ExternalId);
                await _clientMapRepository.SaveAsync(mapping);
            }
            else
            {
                mapping.MapToExternalId(command.ExternalId);
                await _clientMapRepository.SaveAsync(mapping, mapping.Id);
            }

            return new Unit();
        }
    }
}
