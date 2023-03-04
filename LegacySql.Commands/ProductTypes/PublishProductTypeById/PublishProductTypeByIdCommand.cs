using MediatR;

namespace LegacySql.Commands.ProductTypes.PublishProductTypeById
{
    public class PublishProductTypeByIdCommand : IRequest
    {
        public PublishProductTypeByIdCommand(int productTypeId)
        {
            ProductTypeId = productTypeId;
        }

        public int ProductTypeId { get; }
    }
}
