using MediatR;

namespace LegacySql.Commands.Rejects.PublishRejects
{
    public class PublishRejectsCommand : IRequest
    {
        public PublishRejectsCommand(int? id, bool onlyOpen = false)
        {
            OnlyOpen = onlyOpen;
            Id = id;
        }

        public int? Id { get; }
        public bool OnlyOpen { get; }
    }
}
