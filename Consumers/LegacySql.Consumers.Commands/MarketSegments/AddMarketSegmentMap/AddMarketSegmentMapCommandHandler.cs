using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.MarketSegments;
using MediatR;

namespace LegacySql.Consumers.Commands.MarketSegments.AddMarketSegmentMap
{
    public class AddMarketSegmentMapCommandHandler  : IRequestHandler<AddMarketSegmentMapCommand>
    {
        private readonly IMarketSegmentMapRepository _marketSegmentMapRepository;

        public AddMarketSegmentMapCommandHandler(IMarketSegmentMapRepository marketSegmentMapRepository)
        {
            _marketSegmentMapRepository = marketSegmentMapRepository;
        }

        public async Task<Unit> Handle(AddMarketSegmentMapCommand command, CancellationToken cancellationToken)
        {
            var employeeMap = await _marketSegmentMapRepository.GetByMapAsync(command.MessageId);
            if (employeeMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            employeeMap.MapToExternalId(command.ExternalMapId);
            await _marketSegmentMapRepository.SaveAsync(employeeMap, employeeMap.Id);

            return new Unit();
        }
    }
}
