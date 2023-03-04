using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace LegacySql.Commands.PriceAlgorythm.SaveErpPriceAlgorythm
{
    public class SaveErpPriceAlgorythmCommandHandler : IRequestHandler<SaveErpPriceAlgorythmCommand, int>
    {
        private ErpPriceAlgorythmSaver _erpPriceAlgorythmSaver;
        
        public SaveErpPriceAlgorythmCommandHandler(ErpPriceAlgorythmSaver erpPriceAlgorythmSaver)
        {
            _erpPriceAlgorythmSaver = erpPriceAlgorythmSaver;
        }

        public async Task<int> Handle(SaveErpPriceAlgorythmCommand command, CancellationToken cancellationToken)
        {
            var priceAlgorythm = command.Value;

            if (command.Id.HasValue && command.Id <= 0)
            {
                throw new ArgumentException("Id must be >= 0!");
            }
            
            _erpPriceAlgorythmSaver.InitErpObject(priceAlgorythm);

            var mapInfo = await _erpPriceAlgorythmSaver.GetMappingInfo();
            if (!mapInfo.IsMappingFull)
            {
                throw new ArgumentException(mapInfo.Why);
            }

            return await _erpPriceAlgorythmSaver.SaveErpObject(command.Id);
        }
    }
}