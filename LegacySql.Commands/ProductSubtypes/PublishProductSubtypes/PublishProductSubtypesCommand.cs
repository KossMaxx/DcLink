using MediatR;

namespace LegacySql.Commands.ProductSubtypes.PublishProductSubtypes
{
    public class PublishProductSubtypesCommand : IRequest
    {
        public PublishProductSubtypesCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
