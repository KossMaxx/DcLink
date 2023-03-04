using MediatR;

namespace LegacySql.Commands.Clients.PublishClients
{
    public class PublishClientsCommand : IRequest
    {
        public PublishClientsCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
