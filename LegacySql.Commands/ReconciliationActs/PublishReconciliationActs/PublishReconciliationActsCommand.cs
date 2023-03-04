using MediatR;

namespace LegacySql.Commands.ReconciliationActs.PublishReconciliationActs
{
    public class PublishReconciliationActsCommand : IRequest
    {
        public PublishReconciliationActsCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
