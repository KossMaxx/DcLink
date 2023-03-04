using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductRefunds;
using MediatR;

namespace LegacySql.Consumers.Commands.ProductRefunds.AddProductRefundMap
{
    public class AddProductRefundMapCommandHandler : IRequestHandler<AddProductRefundMapCommand>
    {
        private readonly IProductRefundMapRepository _productRefundMapRepository;

        public AddProductRefundMapCommandHandler(IProductRefundMapRepository productRefundMapRepository)
        {
            _productRefundMapRepository = productRefundMapRepository;
        }


        public async Task<Unit> Handle(AddProductRefundMapCommand command, CancellationToken cancellationToken)
        {
            var productRefundMap = await _productRefundMapRepository.GetByMapAsync(command.MessageId);
            if (productRefundMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            productRefundMap.MapToExternalId(command.ExternalMapId);
            await _productRefundMapRepository.SaveAsync(productRefundMap, productRefundMap.Id);

            return new Unit();
        }
    }
}
