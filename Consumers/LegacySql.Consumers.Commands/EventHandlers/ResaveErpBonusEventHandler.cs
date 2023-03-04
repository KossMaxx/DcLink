using System.Threading;
using System.Threading.Tasks;
using LegacySql.Consumers.Commands.Bonuses;
using LegacySql.Consumers.Commands.Events;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Consumers.Commands.EventHandlers
{
    public class ResaveErpBonusEventHandler : INotificationHandler<ResaveErpBonusEvent>
    {
        private readonly ErpBonusSaver _erpBonusSaver;
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;

        public ResaveErpBonusEventHandler(
            ErpBonusSaver erpBonusSaver, 
            IErpNotFullMappedRepository erpNotFullMappedRepository)
        {
            _erpBonusSaver = erpBonusSaver;
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
        }

        public async Task Handle(ResaveErpBonusEvent notification, CancellationToken cancellationToken)
        {
            foreach (var bonus in notification.Messages)
            {
                if (bonus.Sum < 0)
                {
                    bonus.Sum = -bonus.Sum;
                }

                _erpBonusSaver.InitErpObject(bonus);

                var mapInfo = await _erpBonusSaver.GetMappingInfo();
                if (!mapInfo.IsMappingFull)
                {
                    continue;
                }
                await _erpBonusSaver.Create();
                await _erpNotFullMappedRepository.RemoveAsync(bonus.Id, MappingTypes.Bonus);
            }
        }
    }
}