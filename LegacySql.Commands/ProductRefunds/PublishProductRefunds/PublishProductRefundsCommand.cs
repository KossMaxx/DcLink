using MediatR;

namespace LegacySql.Commands.ProductRefunds.PublishProductRefunds
{
    public class PublishProductRefundsCommand : IRequest
    {
        public PublishProductRefundsCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}