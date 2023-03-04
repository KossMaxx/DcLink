using LegacySql.Domain.Deliveries;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.Deliveries.AddDeliveryMap
{
    public class AddDeliveryMapCommandHandler : IRequestHandler<AddDeliveryMapCommand>
    {
        private readonly IDeliveryMapRepository _deliveryMapRepository;

        public AddDeliveryMapCommandHandler(IDeliveryMapRepository deliveryMapRepository)
        {
            _deliveryMapRepository = deliveryMapRepository;
        }
        public async Task<Unit> Handle(AddDeliveryMapCommand command, CancellationToken cancellationToken)
        {
            var deliveryMap = await _deliveryMapRepository.GetByMapAsync(command.MessageId);
            if (deliveryMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            deliveryMap.MapToExternalId(command.ExternalMapId);
            await _deliveryMapRepository.SaveAsync(deliveryMap, deliveryMap.Id);

            return new Unit();
        }
    }
}
