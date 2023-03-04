using MediatR;

namespace LegacySql.Commands.Deliveries.PublishDeliveries
{
    public class PublishDeliveryCommand : IRequest
    {
        public int? Id { get; }

        public PublishDeliveryCommand(int? id)
        {
            Id = id;
        }
    }
}
