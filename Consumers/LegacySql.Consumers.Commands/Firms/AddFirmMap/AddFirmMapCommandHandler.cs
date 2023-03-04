using LegacySql.Domain.Firms;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.Firms.AddFirmMap
{
    public class AddFirmMapCommandHandler : IRequestHandler<AddFirmMapCommand>
    {
        private readonly IFirmMapRepository _firmMapRepository;

        public AddFirmMapCommandHandler(IFirmMapRepository firmMapRepository)
        {
            _firmMapRepository = firmMapRepository;
        }

        public async Task<Unit> Handle(AddFirmMapCommand command, CancellationToken cancellationToken)
        {
            var firmMap = await _firmMapRepository.GetByMapAsync(command.MessageId);
            if (firmMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            firmMap.MapToExternalId(command.ExternalMapId);
            await _firmMapRepository.SaveAsync(firmMap, firmMap.Id);

            return new Unit();
        }
    }
}
