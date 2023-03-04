using LegacySql.Domain.PartnerProductGroups;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.PartnerProductGroups.AddPartnerProductGroup
{
    public class AddPartnerProductGroupCommandHandler : IRequestHandler<AddPartnerProductGroupCommand>
    {
        private readonly IPartnerProductGroupsMapRepository _partnerProductGropMapRepository;

        public AddPartnerProductGroupCommandHandler(IPartnerProductGroupsMapRepository partnerProductGropMapRepository)
        {
            _partnerProductGropMapRepository = partnerProductGropMapRepository;
        }

        public async Task<Unit> Handle(AddPartnerProductGroupCommand command, CancellationToken cancellationToken)
        {
            var entityMap = await _partnerProductGropMapRepository.GetByMapAsync(command.MessageId);
            if (entityMap == null)
            {
                throw new KeyNotFoundException($"Id сообщения  {command.MessageId} не найден");
            }

            entityMap.MapToExternalId(command.ExternalMapId);
            await _partnerProductGropMapRepository.SaveAsync(entityMap, entityMap.Id);

            return new Unit();
        }
    }
}
