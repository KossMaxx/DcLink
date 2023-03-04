using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Rejects;
using MediatR;

namespace LegacySql.Consumers.Commands.Rejects.AddRejectMap
{
    public class AddRejectMapCommandHandler : IRequestHandler<AddRejectMapCommand>
    {
        private readonly IRejectMapRepository _rejectMapRepository;

        public AddRejectMapCommandHandler(IRejectMapRepository rejectMapRepository)
        {
            _rejectMapRepository = rejectMapRepository;
        }

        public async Task<Unit> Handle(AddRejectMapCommand command, CancellationToken cancellationToken)
        {
            var rejectMap = await _rejectMapRepository.GetByMapAsync(command.MessageId);
            if (rejectMap == null)
            {
                return new Unit();
            }

            rejectMap.MapToExternalId(command.ExternalMapId);
            await _rejectMapRepository.SaveAsync(rejectMap, rejectMap.Id);

            return new Unit();
        }
    }
}