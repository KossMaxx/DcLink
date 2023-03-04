using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Cashboxes;
using MediatR;

namespace LegacySql.Commands.Cashboxes.SetCashboxHandMapping
{
    public class SetCashboxHandMappingCommandHandler : IRequestHandler<SetCashboxHandMappingCommand>
    {
        private readonly ICashboxMapRepository _cashboxMapRepository;
        private readonly ILegacyCashboxRepository _legacyCashboxRepository;

        public SetCashboxHandMappingCommandHandler(ICashboxMapRepository cashboxMapRepository, ILegacyCashboxRepository legacyCashboxRepository)
        {
            _cashboxMapRepository = cashboxMapRepository;
            _legacyCashboxRepository = legacyCashboxRepository;
        }

        public async Task<Unit> Handle(SetCashboxHandMappingCommand command, CancellationToken cancellationToken)
        {
            var cashboxMap = await _cashboxMapRepository.GetByLegacyAsync(command.InnerId);
            if (cashboxMap != null && cashboxMap.Id == command.ExternalId)
            {
                return new Unit();
            }
            else
            {
                var legacyCashbox = await _legacyCashboxRepository.Get(command.InnerId, cancellationToken);

                if (legacyCashbox == null)
                {
                    throw new KeyNotFoundException($"Касса с Id: {command.InnerId} не найден");
                }

                cashboxMap = new CashboxMap(Guid.NewGuid(), command.InnerId, legacyCashbox.Description, command.ExternalId);
                await _cashboxMapRepository.SaveAsync(cashboxMap);
            }
            
            return new Unit();
        }
    }
}
