using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Manufacturer;
using MediatR;

namespace LegacySql.Consumers.Commands.Manufacturers.AddManufacturerMap
{
    public class AddManufacturerMapCommandHandler : IRequestHandler<AddManufacturerMapCommand>
    {
        private readonly IManufacturerMapRepository _manufacturerMapRepository;

        public AddManufacturerMapCommandHandler(IManufacturerMapRepository manufacturerMapRepository)
        {
            _manufacturerMapRepository = manufacturerMapRepository;
        }

        public async Task<Unit> Handle(AddManufacturerMapCommand command, CancellationToken cancellationToken)
        {
            var map = await _manufacturerMapRepository.GetByMapAsync(command.MessageId);
            if (map == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            map.MapToExternalId(command.ExternalMapId);
            await _manufacturerMapRepository.SaveAsync(map, map.Id);

            return new Unit();
        }
    }
}
