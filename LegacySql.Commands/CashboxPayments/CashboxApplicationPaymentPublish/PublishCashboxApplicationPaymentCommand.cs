using MediatR;

namespace LegacySql.Commands.CashboxPayments.CashboxApplicationPaymentPublish
{
    public class PublishCashboxApplicationPaymentCommand : IRequest
    {
        public PublishCashboxApplicationPaymentCommand(int? id)
        {
            Id = id;
        }

        public int? Id { get; }
    }
}
