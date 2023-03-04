using LegacySql.Domain.ProductMoving;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.ProductMovings.AddProductMovingMap
{
    public class AddProductMovingMapCommandHandler : IRequestHandler<AddProductMovingMapCommand>
    {
        private readonly IProductMovingMapRepository _productMovingMapRepository;

        public AddProductMovingMapCommandHandler(IProductMovingMapRepository productMovingMapRepository)
        {
            _productMovingMapRepository = productMovingMapRepository;
        }

        public async Task<Unit> Handle(AddProductMovingMapCommand command, CancellationToken cancellationToken)
        {
            var map = await _productMovingMapRepository.GetByMapAsync(command.MessageId);
            if (map == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            map.MapToExternalId(command.ExternalMapId);
            await _productMovingMapRepository.SaveAsync(map, map.Id);

            return new Unit();
        }
    }
}
