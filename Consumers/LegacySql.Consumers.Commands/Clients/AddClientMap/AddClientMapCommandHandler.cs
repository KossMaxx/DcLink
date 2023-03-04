using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Clients;
using MediatR;

namespace LegacySql.Consumers.Commands.Clients.AddClientMap
{
    public class AddClientMapCommandHandler : IRequestHandler<AddClientMapCommand>
    {
        private readonly IClientMapRepository _clientMapRepository;

        public AddClientMapCommandHandler(IClientMapRepository clientMapRepository)
        {
            _clientMapRepository = clientMapRepository;
        }

        public async Task<Unit> Handle(AddClientMapCommand command, CancellationToken cancellationToken)
        {
            var clientMap = await _clientMapRepository.GetByMapAsync(command.MessageId);
            if (clientMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            clientMap.MapToExternalId(command.ExternalMapId);
            await _clientMapRepository.SaveAsync(clientMap, clientMap.Id);

            return new Unit();
        }
    }
}
