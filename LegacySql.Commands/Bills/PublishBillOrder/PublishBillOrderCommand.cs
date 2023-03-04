using MediatR;

namespace LegacySql.Commands.Bills.PublishBillOrder
{
    public class PublishBillOrderCommand : IRequest
    {
        public PublishBillOrderCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
