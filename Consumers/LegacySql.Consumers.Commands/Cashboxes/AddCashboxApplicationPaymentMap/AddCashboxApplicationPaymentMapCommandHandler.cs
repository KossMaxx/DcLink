using LegacySql.Domain.Cashboxes;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.Cashboxes.AddCashboxApplicationPaymentMap
{
    internal class AddCashboxApplicationPaymentMapCommandHandler : IRequestHandler<AddCashboxApplicationPaymentMapCommand>
    {
        private ICashboxApplicationPaymentMapRepository _cashboxApplicationPaymentMapRepository;

        public AddCashboxApplicationPaymentMapCommandHandler(ICashboxApplicationPaymentMapRepository cashboxApplicationPaymentMapRepository)
        {
            _cashboxApplicationPaymentMapRepository = cashboxApplicationPaymentMapRepository;
        }
        public async Task<Unit> Handle(AddCashboxApplicationPaymentMapCommand command, CancellationToken cancellationToken)
        {
            var paymentMap = await _cashboxApplicationPaymentMapRepository.GetByMapAsync(command.MessageId);
            if (paymentMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            paymentMap.MapToExternalId(command.ExternalMapId);
            await _cashboxApplicationPaymentMapRepository.SaveAsync(paymentMap, paymentMap.Id);

            return new Unit();
        }
    }
}
