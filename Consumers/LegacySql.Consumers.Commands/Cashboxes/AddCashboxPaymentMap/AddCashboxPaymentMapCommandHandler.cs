using LegacySql.Domain.Cashboxes;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.Cashboxes.AddCashboxPaymentMap
{
    public class AddCashboxPaymentMapCommandHandler : IRequestHandler<AddCashboxPaymentMapCommand>
    {
        private ICashboxPaymentMapRepository _cashboxPaymentMapRepository;

        public AddCashboxPaymentMapCommandHandler(ICashboxPaymentMapRepository cashboxPaymentMapRepository)
        {
            _cashboxPaymentMapRepository = cashboxPaymentMapRepository;
        }

        public async Task<Unit> Handle(AddCashboxPaymentMapCommand command, CancellationToken cancellationToken)
        {
            var paymentMap = await _cashboxPaymentMapRepository.GetByMapAsync(command.MessageId);
            if (paymentMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            paymentMap.MapToExternalId(command.ExternalMapId);
            await _cashboxPaymentMapRepository.SaveAsync(paymentMap, paymentMap.Id);

            return new Unit();
        }
    }
}
