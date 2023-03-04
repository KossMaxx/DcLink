using LegacySql.Domain.Bills;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.Bills.AddBillMap
{
    public class AddBillMapCommandHandler : IRequestHandler<AddBillMapCommand>
    {
        private readonly IBillMapRepository _billMapRepository;

        public AddBillMapCommandHandler(IBillMapRepository billMapRepository)
        {
            _billMapRepository = billMapRepository;
        }

        public async Task<Unit> Handle(AddBillMapCommand command, CancellationToken cancellationToken)
        {
            var billMap = await _billMapRepository.GetByMapAsync(command.MessageId);
            if (billMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            billMap.MapToExternalId(command.ExternalMapId);
            await _billMapRepository.SaveAsync(billMap, billMap.Id);

            return new Unit();
        }
    }
}
