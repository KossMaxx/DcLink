using LegacySql.Domain.Cashboxes;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Commands.CashboxPayments.CarryingOutCashboxApplicationPayments
{
    public class CarryingOutCashboxApplicationPaymentCommandHandler : IRequestHandler<CarryingOutCashboxApplicationPaymentCommand>
    {
        private readonly ILegacyCashboxApplicationPaymentRepository _legacyCashboxApplicationPaymentRepository;

        public CarryingOutCashboxApplicationPaymentCommandHandler(ILegacyCashboxApplicationPaymentRepository legacyCashboxApplicationPaymentRepository)
        {
            _legacyCashboxApplicationPaymentRepository = legacyCashboxApplicationPaymentRepository;
        }

        public async Task<Unit> Handle(CarryingOutCashboxApplicationPaymentCommand command, CancellationToken cancellationToken)
        {
            await _legacyCashboxApplicationPaymentRepository.CarryingOutCashboxApplicationPayment(command.Id, command.IncomePaymentId, command.OutPaymentId, command.UserId, command.Date, command.HeldIn, cancellationToken);

            return new Unit();
        }
    }
}
