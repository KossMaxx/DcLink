using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.PhysicalPersons;
using MediatR;

namespace LegacySql.Consumers.Commands.PhysicalPersons.AddPhysicalPersonMap
{
    public class AddPhysicalPersonMapCommandHandler : IRequestHandler<AddPhysicalPersonMapCommand>
    {
        private IPhysicalPersonMapRepository _physicalPersonMapRepository;

        public AddPhysicalPersonMapCommandHandler(IPhysicalPersonMapRepository physicalPersonMapRepository)
        {
            _physicalPersonMapRepository = physicalPersonMapRepository;
        }

        public async Task<Unit> Handle(AddPhysicalPersonMapCommand command, CancellationToken cancellationToken)
        {
            var physicalPersonMap = await _physicalPersonMapRepository.GetByMapAsync(command.MessageId);
            if (physicalPersonMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            physicalPersonMap.MapToExternalId(command.ExternalMapId);
            await _physicalPersonMapRepository.SaveAsync(physicalPersonMap, physicalPersonMap.Id);

            return new Unit();
        }
    }
}
