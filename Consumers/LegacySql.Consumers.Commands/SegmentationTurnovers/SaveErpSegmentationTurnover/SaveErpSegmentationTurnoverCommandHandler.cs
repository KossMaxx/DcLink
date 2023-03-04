using LegacySql.Domain.SegmentationTurnovers;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.ConsumerCommandContracts;
using MediatR;
using MessageBus.SegmentationTurnovers.Import;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.SegmentationTurnovers.SaveErpSegmentationTurnover
{
    internal class SaveErpSegmentationTurnoverCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpSegmentationTurnoverDto>>
    {
        private readonly ISegmentationTurnoversMapRepository _segmentationTurnoversMapRepository;
        private ExternalMap _turnoverMap;
        private readonly ISegmentationTurnoverStore _store;

        public SaveErpSegmentationTurnoverCommandHandler(ISegmentationTurnoversMapRepository segmentationTurnoversMapRepository, ISegmentationTurnoverStore store)
        {
            _segmentationTurnoversMapRepository = segmentationTurnoversMapRepository;
            _store = store;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpSegmentationTurnoverDto> command, CancellationToken cancellationToken)
        {
            var turnover = command.Value;
            _turnoverMap = await _segmentationTurnoversMapRepository.GetByErpAsync(turnover.Id);
            if (_turnoverMap == null)
            {
                var newId = await Create(turnover);
                await _segmentationTurnoversMapRepository.SaveAsync(new ExternalMap(command.MessageId, newId, turnover.Id));
            }
            else
            {
                await Update(turnover);
            }

            return new Unit();
        }

        private async Task Update(ErpSegmentationTurnoverDto turnover)
        {
            await _store.Update(_turnoverMap.LegacyId, turnover.Title);
        }

        private async Task<int> Create(ErpSegmentationTurnoverDto turnover)
        {
            return await _store.Create(turnover.Title);
        }
    }
}
