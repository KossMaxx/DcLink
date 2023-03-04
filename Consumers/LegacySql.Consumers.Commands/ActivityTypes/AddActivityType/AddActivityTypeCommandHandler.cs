using LegacySql.Domain.ActivityTypes;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.ActivityTypes.AddActivityType
{
    public class AddActivityTypeCommandHandler : IRequestHandler<AddActivityTypeCommand>
    {
        private readonly IActivityTypesMapRepository _activityTypeMapRepository;

        public AddActivityTypeCommandHandler(IActivityTypesMapRepository activityTypeMapRepository)
        {
            _activityTypeMapRepository = activityTypeMapRepository;
        }

        public async Task<Unit> Handle(AddActivityTypeCommand command, CancellationToken cancellationToken)
        {
            var entityMap = await _activityTypeMapRepository.GetByMapAsync(command.MessageId);
            if (entityMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            entityMap.MapToExternalId(command.ExternalMapId);
            await _activityTypeMapRepository.SaveAsync(entityMap, entityMap.Id);

            return new Unit();
        }
    }
}
