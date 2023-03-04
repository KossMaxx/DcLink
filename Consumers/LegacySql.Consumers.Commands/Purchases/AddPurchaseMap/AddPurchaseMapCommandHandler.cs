using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Purchases;
using MediatR;

namespace LegacySql.Consumers.Commands.Purchases.AddPurchaseMap
{
    public class AddPurchaseMapCommandHandler : IRequestHandler<AddPurchaseMapCommand>
    {
        private readonly IPurchaseMapRepository _purchaseMapRepository;

        public AddPurchaseMapCommandHandler(IPurchaseMapRepository purchaseMapRepository)
        {
            _purchaseMapRepository = purchaseMapRepository;
        }


        public async Task<Unit> Handle(AddPurchaseMapCommand command, CancellationToken cancellationToken)
        {
            var purchaseMap = await _purchaseMapRepository.GetByMapAsync(command.MessageId);
            if (purchaseMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            purchaseMap.MapToExternalId(command.ExternalMapId);
            await _purchaseMapRepository.SaveAsync(purchaseMap, purchaseMap.Id);

            return new Unit();
        }
    }
}
