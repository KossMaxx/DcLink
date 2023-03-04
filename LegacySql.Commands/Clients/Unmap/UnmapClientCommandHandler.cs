using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Clients;
using MediatR;

namespace LegacySql.Commands.Clients.Unmap
{
    public class UnmapClientCommandHandler : IRequestHandler<UnmapClientCommand>
    {
        private readonly IClientMapRepository _clientMapRepository;

        public UnmapClientCommandHandler(IClientMapRepository clientMapRepository)
        {
            _clientMapRepository = clientMapRepository;
        }

        public async Task<Unit> Handle(UnmapClientCommand command, CancellationToken cancellationToken)
        {
            var mapping = await _clientMapRepository.GetByErpAsync(command.ErpId);
            await _clientMapRepository.DeleteByIdAsync(mapping.Id);

            return new Unit();
        }
    }
}
