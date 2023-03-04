using MediatR;

namespace LegacySql.Commands.ProductPriceConditions.PublishProductPriceConditions
{
    public class PublishProductPriceConditionsCommand : IRequest
    {
        public PublishProductPriceConditionsCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}