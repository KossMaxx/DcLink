using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.ProductTypeCategoryParameters;
using MediatR;

namespace LegacySql.Consumers.Commands.ProductTypes.AddProductTypeCategoryParameterMap
{
    public class AddProductTypeCategoryParameterMapCommandHandler : IRequestHandler<AddProductTypeCategoryParameterMapCommand>
    {
        private readonly IProductTypeCategoryParameterMapRepository _productTypeCategoryParameterMapRepository;

        public AddProductTypeCategoryParameterMapCommandHandler(IProductTypeCategoryParameterMapRepository productTypeCategoryParameterMapRepository)
        {
            _productTypeCategoryParameterMapRepository = productTypeCategoryParameterMapRepository;
        }

        public async Task<Unit> Handle(AddProductTypeCategoryParameterMapCommand command, CancellationToken cancellationToken)
        {
            var productTypeCategoryParameterMap = await _productTypeCategoryParameterMapRepository.GetByMapAsync(command.MessageId);
            if (productTypeCategoryParameterMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            productTypeCategoryParameterMap.MapToExternalId(command.ExternalMapId);
            await _productTypeCategoryParameterMapRepository.SaveAsync(productTypeCategoryParameterMap, productTypeCategoryParameterMap.Id);

            return new Unit();
        }
    }
}
