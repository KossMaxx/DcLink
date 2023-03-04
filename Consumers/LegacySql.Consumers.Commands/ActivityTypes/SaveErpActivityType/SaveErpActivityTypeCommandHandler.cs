using LegacySql.Domain.ActivityTypes;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.ConsumerCommandContracts;
using MediatR;
using MessageBus.ActivityTypes.Import;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.ActivityTypes.SaveErpActivityType
{
    public class SaveErpActivityTypeCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpActivityTypeDto>>
    {
        private readonly IActivityTypesMapRepository _activityTypesMapRepository;
        private ExternalMap _typeMap;
        private readonly IActivityTypeStore _store;

        public SaveErpActivityTypeCommandHandler(IActivityTypesMapRepository activityTypesMapRepository, IActivityTypeStore store)
        {
            _activityTypesMapRepository = activityTypesMapRepository;
            _store = store;
        }


        public async Task<Unit> Handle(BaseSaveErpCommand<ErpActivityTypeDto> command, CancellationToken cancellationToken)
        {
            var type = command.Value;
            _typeMap = await _activityTypesMapRepository.GetByErpAsync(type.Id);
            if (_typeMap == null)
            {
                var newGroupId = await Create(type);
                await _activityTypesMapRepository.SaveAsync(new ExternalMap(command.MessageId, newGroupId, type.Id));
            }
            else
            {
                await Update(type);
            }

            return new Unit();
        }
        private async Task Update(ErpActivityTypeDto type)
        {
            await _store.Update(_typeMap.LegacyId, type.Title);
        }

        private async Task<int> Create(ErpActivityTypeDto type)
        {
            return await _store.Create(type.Title);
        }
    }
}
