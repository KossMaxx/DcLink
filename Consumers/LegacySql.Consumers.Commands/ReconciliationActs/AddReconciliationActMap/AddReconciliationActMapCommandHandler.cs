using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ReconciliationActs;
using MediatR;

namespace LegacySql.Consumers.Commands.ReconciliationActs.AddReconciliationActMap
{
    public class AddReconciliationActMapCommandHandler : IRequestHandler<AddReconciliationActMapCommand>
    {
        private readonly IReconciliationActMapRepository _reconciliationActMapRepository;

        public AddReconciliationActMapCommandHandler(IReconciliationActMapRepository reconciliationActMapRepository)
        {
            _reconciliationActMapRepository = reconciliationActMapRepository;
        }

        public async Task<Unit> Handle(AddReconciliationActMapCommand command, CancellationToken cancellationToken)
        {
            var reconciliationActMap = await _reconciliationActMapRepository.GetByMapAsync(command.MessageId);
            if (reconciliationActMap == null)
            {
                return new Unit();
            }

            reconciliationActMap.MapToExternalId(command.ExternalMapId);
            await _reconciliationActMapRepository.SaveAsync(reconciliationActMap, reconciliationActMap.Id);

            return new Unit();
        }
    }
}