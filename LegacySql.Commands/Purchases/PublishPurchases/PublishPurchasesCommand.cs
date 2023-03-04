using MediatR;

namespace LegacySql.Commands.Purchases.PublishPurchases
{
    public class PublishPurchasesCommand : IRequest
    {
        public PublishPurchasesCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}