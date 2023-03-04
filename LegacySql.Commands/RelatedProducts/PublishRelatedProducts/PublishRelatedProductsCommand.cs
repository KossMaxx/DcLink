using MediatR;

namespace LegacySql.Commands.RelatedProducts.PublishRelatedProducts
{
    public class PublishRelatedProductsCommand : IRequest
    {
        public PublishRelatedProductsCommand(int? id )
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
