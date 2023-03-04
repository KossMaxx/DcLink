using MediatR;

namespace LegacySql.Commands.ClientOrders.PublishClientOrderById
{
    public class PublishClientOrderByIdCommand : IRequest
    {
        public int Id { get; }

        public PublishClientOrderByIdCommand(int id)
        {
            Id = id;
        }
    }
}
