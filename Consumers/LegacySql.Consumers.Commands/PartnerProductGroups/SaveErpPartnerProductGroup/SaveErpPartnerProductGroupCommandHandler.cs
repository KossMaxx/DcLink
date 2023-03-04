using LegacySql.Domain.PartnerProductGroups;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.ConsumerCommandContracts;
using MediatR;
using MessageBus.PartnerProductGroups.Import;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.PartnerProductGroups.SaveErpPartnerProductGroup
{
    internal class SaveErpPartnerProductGroupCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpPartnerProductGroupDto>>
    {
        private readonly IPartnerProductGroupsMapRepository _partnerProductGroupsMapRepository;
        private ExternalMap _groupMap;
        private readonly IPartnerProductGroupsStore _store;

        public SaveErpPartnerProductGroupCommandHandler(
            IPartnerProductGroupsMapRepository partnerProductGroupsMapRepository, 
            IPartnerProductGroupsStore store)
        {
            _partnerProductGroupsMapRepository = partnerProductGroupsMapRepository;
            _store = store;
        }


        public async Task<Unit> Handle(BaseSaveErpCommand<ErpPartnerProductGroupDto> command, CancellationToken cancellationToken)
        {
            var group = command.Value;
            _groupMap = await _partnerProductGroupsMapRepository.GetByErpAsync(group.Id);
            if (_groupMap == null)
            {
                var newGroupId = await Create(group);
                await _partnerProductGroupsMapRepository.SaveAsync(new ExternalMap(command.MessageId, newGroupId, group.Id));
            }
            else
            {
                await Update(group);
            }

            return new Unit();
        }

        private async Task Update(ErpPartnerProductGroupDto group)
        {
            await _store.Update(_groupMap.LegacyId, group.Title);
        }

        private async Task<int> Create(ErpPartnerProductGroupDto group)
        {
            return await _store.Create(group.Title);
        }
    }
}
