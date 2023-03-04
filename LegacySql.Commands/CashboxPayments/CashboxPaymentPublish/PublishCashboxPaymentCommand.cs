using MediatR;

namespace LegacySql.Commands.CashboxPayments.CashboxPaymentPublish
{
    public class PublishCashboxPaymentCommand : IRequest
    {
        public PublishCashboxPaymentCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
