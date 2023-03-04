using MediatR;

namespace LegacySql.Commands.PriceConditions.PublishPriceConditions
{
    public class PublishPriceConditionsCommand : IRequest
    {
        public PublishPriceConditionsCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}