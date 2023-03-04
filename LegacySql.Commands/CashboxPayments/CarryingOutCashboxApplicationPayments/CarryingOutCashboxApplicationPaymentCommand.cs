using MediatR;
using System;

namespace LegacySql.Commands.CashboxPayments.CarryingOutCashboxApplicationPayments
{
    public class CarryingOutCashboxApplicationPaymentCommand : IRequest
    {
        public Guid Id { get; }
        public Guid OutPaymentId { get; }
        public Guid IncomePaymentId { get; }
        public Guid UserId { get; }
        public DateTime Date { get; }
        public bool HeldIn { get; }

        public CarryingOutCashboxApplicationPaymentCommand(Guid id, Guid incomePaymentId, Guid outPaymentId, Guid userId, DateTime date, bool heldIn)
        {
            Id = id;
            OutPaymentId = outPaymentId;
            IncomePaymentId = incomePaymentId;
            UserId = userId;
            Date = date;
            HeldIn = heldIn;
        }
    }
}
