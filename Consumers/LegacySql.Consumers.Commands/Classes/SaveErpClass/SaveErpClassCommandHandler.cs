using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Classes;
using MediatR;
using MessageBus.Classes.Import;

namespace LegacySql.Consumers.Commands.Classes.SaveErpClass
{
    public class SaveErpClassCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpClassDto>>
    {
        private readonly IClassMapRepository _classMapRepository;
        private readonly ErpClassSaver _erpClassSaver;


        public SaveErpClassCommandHandler(IClassMapRepository classMapRepository, ErpClassSaver erpClassSaver)
        {
            _classMapRepository = classMapRepository;
            _erpClassSaver = erpClassSaver;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpClassDto> command, CancellationToken cancellationToken)
        {
            var newClass = command.Value;
            var map = await _classMapRepository.GetByErpAsync(newClass.Id);

            _erpClassSaver.InitErpObject(newClass, map);

            //var mappingInfo = await _erpClassSaver.GetMappingInfo();

            //if (!mappingInfo.IsMappingFull)
            //{
            //    await SaveNotFullMapping(subtype, mappingInfo.Why);
            //    return new Unit();
            //}

            if (map == null)
            {
                await _erpClassSaver.Create(command.MessageId);
              
            }
            else
            {
                await _erpClassSaver.Update(cancellationToken);
            }

            //await _erpNotFullMappedRepository.RemoveAsync(subtype.Id, MappingTypes.ProductSubtype);
            return new Unit();

        }
    }
}

