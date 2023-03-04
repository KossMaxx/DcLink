using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Consumers.Commands.Penalties;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpPenaltyEventHandler : INotificationHandler<ResaveErpPenaltyEvent>
    {
        private readonly ErpPenaltySaver _penaltySaver;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;

        public ResaveErpPenaltyEventHandler(
            ErpPenaltySaver penaltySaver, 
            IErpNotFullMappedRepository erpNotFullMappedRepository)
        {
            _penaltySaver = penaltySaver;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
        }

        public async Task Handle(ResaveErpPenaltyEvent notification, CancellationToken cancellationToken)
        {
            foreach (var penalty in notification.Messages)
            {
                if (penalty.Sum > 0)
                {
                    penalty.Sum = -penalty.Sum;
                }

                _penaltySaver.InitErpObject(penalty);

                var mapInfo = await _penaltySaver.GetMappingInfo();
                if (!mapInfo.IsMappingFull)
                {
                    continue;
                }

                await _penaltySaver.Create();
                await _erpNotFullMappedRepository.RemoveAsync(penalty.Id, MappingTypes.Penalty);
            }
        }
    }
}