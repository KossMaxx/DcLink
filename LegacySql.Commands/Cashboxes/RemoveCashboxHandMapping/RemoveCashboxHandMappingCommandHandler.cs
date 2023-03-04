using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Cashboxes;
using MediatR;

namespace LegacySql.Commands.Cashboxes.RemoveCashboxHandMapping
{
    public class RemoveCashboxHandMappingCommandHandler : IRequestHandler<RemoveCashboxHandMappingCommand>
    {
        private readonly ICashboxMapRepository _cashboxMapRepository;

        public RemoveCashboxHandMappingCommandHandler(ICashboxMapRepository cashboxMapRepository)
        {
            _cashboxMapRepository = cashboxMapRepository;
        }

        public async Task<Unit> Handle(RemoveCashboxHandMappingCommand command, CancellationToken cancellationToken)
        {
            if (command.ErpId.HasValue)
            {
                await _cashboxMapRepository.RemoveByErpAsync(command.ErpId.Value);
            }

            if(command.Id.HasValue)
            {
                await _cashboxMapRepository.RemoveByLegacyAsync(command.Id.Value);
            }
            
            return new Unit();
        }
    }
}
