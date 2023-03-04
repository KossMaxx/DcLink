using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ClientOrders;
using MediatR;

namespace LegacySql.Consumers.Commands.ClientOrders.AddClientOrderMap
{
    public class AddClientOrderMapCommandHandler : IRequestHandler<AddClientOrderMapCommand>
    {
        private readonly IClientOrderMapRepository _clientOrderMapRepository;

        public AddClientOrderMapCommandHandler(IClientOrderMapRepository clientOrderMapRepository)
        {
            _clientOrderMapRepository = clientOrderMapRepository;
        }


        public async Task<Unit> Handle(AddClientOrderMapCommand command, CancellationToken cancellationToken)
        {
            var productMap = await _clientOrderMapRepository.GetByMapAsync(command.MessageId);
            if (productMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            productMap.MapToExternalId(command.ExternalMapId);
            await _clientOrderMapRepository.SaveAsync(productMap, productMap.Id);

            return new Unit();
        }
    }
}