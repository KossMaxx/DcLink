using LegacySql.Domain.FreeDocuments;
using LegacySql.Domain.NotFullMappings;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.FreeDocuments.Import;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.FreeDocuments.SaveErpFreeDocuments
{
    public class SaveErpFreeDocumentCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpFreeDocumentDto>>
    {
        private readonly IErpNotFullMappedRepository _erpNotFullMappedRepository;
        private readonly IFreeDocumentMapRepository _freeDocumentMapRepository;
        private readonly ErpFreeDocumentSaver _erpFreeDocumentSaver;

        public SaveErpFreeDocumentCommandHandler(
            IErpNotFullMappedRepository erpNotFullMappedRepository,
            ErpFreeDocumentSaver erpFreeDocumentSaver, 
            IFreeDocumentMapRepository freeDocumentMapRepository)
        {
            _erpNotFullMappedRepository = erpNotFullMappedRepository;
            _erpFreeDocumentSaver = erpFreeDocumentSaver;
            _freeDocumentMapRepository = freeDocumentMapRepository;
        }


        public async Task<Unit> Handle(BaseSaveErpCommand<ErpFreeDocumentDto> command, CancellationToken cancellationToken)
        {
            var doc = command.Value;
            var docMapping = await _freeDocumentMapRepository.GetByErpAsync(doc.Id);
            _erpFreeDocumentSaver.InitErpObject(doc, docMapping);

            var mappingInfo = await _erpFreeDocumentSaver.GetMappingInfo();
            if (!mappingInfo.IsMappingFull)
            {
                await SaveNotFullMapping(doc, mappingInfo.Why);
                return new Unit();
            }

            if (docMapping == null)
            {
                await _erpFreeDocumentSaver.Create(command.MessageId);
            }
            else
            {
                await _erpFreeDocumentSaver.Update();
            }
            await _erpNotFullMappedRepository.RemoveAsync(doc.Id, MappingTypes.FreeDocument);

            return new Unit();
        }

        private async Task SaveNotFullMapping(ErpFreeDocumentDto doc, string why)
        {
            await _erpNotFullMappedRepository.SaveAsync(new ErpNotFullMapped(
                doc.Id,
                MappingTypes.FreeDocument,
                DateTime.Now,
                why,
                JsonConvert.SerializeObject(doc)
            ));
        }
    }
}
