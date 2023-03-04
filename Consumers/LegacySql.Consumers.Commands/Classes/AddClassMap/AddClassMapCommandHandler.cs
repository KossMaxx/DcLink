using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Classes;
using MediatR;

namespace LegacySql.Consumers.Commands.Classes.AddClassMap
{
    public class AddClassMapCommandHandler : IRequestHandler<AddClassMapCommand>
    {
        private readonly IClassMapRepository _classMapRepository;

        public AddClassMapCommandHandler(IClassMapRepository classMapRepository)
        {
            _classMapRepository = classMapRepository;
        }

        public async Task<Unit> Handle(AddClassMapCommand command, CancellationToken cancellationToken)
        {
            var map = await _classMapRepository.GetByMapAsync(command.MessageId);
            if (map == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            map.MapToExternalId(command.ExternalMapId);
            await _classMapRepository.SaveAsync(map, map.Id);

            return new Unit();
        }
    }
}
