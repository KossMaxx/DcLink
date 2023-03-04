using LegacySql.Domain.IncomingBills;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.IncomingBills.AddIncomingBillMap
{
    public class AddIncomingBillMapCommandHandler : IRequestHandler<AddIncomingBillMapCommand>
    {
        private readonly IIncomingBillMapRepository _incomingBillMapRepository;

        public AddIncomingBillMapCommandHandler(IIncomingBillMapRepository incomingBillMapRepository)
        {
            _incomingBillMapRepository = incomingBillMapRepository;
        }

        public async Task<Unit> Handle(AddIncomingBillMapCommand command, CancellationToken cancellationToken)
        {
            var billMap = await _incomingBillMapRepository.GetByMapAsync(command.MessageId);
            if (billMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            billMap.MapToExternalId(command.ExternalMapId);
            await _incomingBillMapRepository.SaveAsync(billMap, billMap.Id);

            return new Unit();
        }
    }
}
