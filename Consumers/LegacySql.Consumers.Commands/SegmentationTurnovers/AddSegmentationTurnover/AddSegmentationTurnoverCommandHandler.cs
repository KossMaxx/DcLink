using LegacySql.Domain.SegmentationTurnovers;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.SegmentationTurnovers.AddSegmentationTurnover
{
    internal class AddSegmentationTurnoverCommandHandler : IRequestHandler<AddSegmentationTurnoverCommand>
    {
        private readonly ISegmentationTurnoversMapRepository _segmentationTurnoversMapRepository;

        public AddSegmentationTurnoverCommandHandler(ISegmentationTurnoversMapRepository segmentationTurnoversMapRepository)
        {
            _segmentationTurnoversMapRepository = segmentationTurnoversMapRepository;
        }

        public async Task<Unit> Handle(AddSegmentationTurnoverCommand command, CancellationToken cancellationToken)
        {
            var entityMap = await _segmentationTurnoversMapRepository.GetByMapAsync(command.MessageId);
            if (entityMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            entityMap.MapToExternalId(command.ExternalMapId);
            await _segmentationTurnoversMapRepository.SaveAsync(entityMap, entityMap.Id);

            return new Unit();
        }
    }
}
