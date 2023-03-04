using MediatR;

namespace LegacySql.Commands.ProductMovings.PublishProductMovings
{
    public class PublishProductMovingCommand : IRequest
    {
        public PublishProductMovingCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
