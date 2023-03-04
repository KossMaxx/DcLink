using MediatR;

namespace LegacySql.Commands.Products.PublishProducts
{
    public class PublishProductsCommand : IRequest
    {
        public PublishProductsCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
