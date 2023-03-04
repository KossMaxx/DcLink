using MediatR;

namespace LegacySql.Commands.Firms.PublishFirms
{
    public class PublishFirmsCommand : IRequest
    {
        public PublishFirmsCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
