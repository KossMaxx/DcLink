using MediatR;

namespace LegacySql.Commands.ClientOrders.PublishClientOrder
{
    public class PublishClientOrderCommand : IRequest
    {
        public PublishClientOrderCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
