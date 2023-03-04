using MediatR;

namespace LegacySql.Commands.Bills.PublishIncomingBills
{
    public class PublishIncomingBillCommand : IRequest
    {
        public PublishIncomingBillCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
