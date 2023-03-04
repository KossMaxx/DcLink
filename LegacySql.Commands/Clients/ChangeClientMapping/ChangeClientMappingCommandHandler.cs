using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Clients;
using MediatR;

namespace LegacySql.Commands.Clients.ChangeClientMapping
{
    public class ChangeClientMappingCommandHandler : IRequestHandler<ChangeClientMappingCommand>
    {
        private readonly IClientMapRepository _clientMapRepository;

        public ChangeClientMappingCommandHandler(IClientMapRepository clientMapRepository)
        {
            _clientMapRepository = clientMapRepository;
        }

        public async Task<Unit> Handle(ChangeClientMappingCommand command, CancellationToken cancellationToken)
        {
            var mapping = await _clientMapRepository.GetByErpAsync(command.OldId);
            if (mapping == null)
            {
                return new Unit();
            }

            mapping.MapToExternalId(command.NewId);
            await _clientMapRepository.SaveAsync(mapping, mapping.Id);

            return new Unit();
        }
    }
}
