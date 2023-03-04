using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.FreeDocuments;
using LegacySql.Domain.Shared;
using MessageBus.FreeDocuments.Import;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacySql.Consumers.Commands.FreeDocuments
{
    public class ErpFreeDocumentSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IFreeDocumentMapRepository _freeDocumentMapRepository;
        private ErpFreeDocumentDto _doc;
        private ExternalMap _clientMapping;
        private ExternalMap _docMapping;

        public ErpFreeDocumentSaver(IDbConnection db, IClientMapRepository clientMapRepository, IFreeDocumentMapRepository freeDocumentMapRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _freeDocumentMapRepository = freeDocumentMapRepository;
        }

        internal void InitErpObject(ErpFreeDocumentDto doc, ExternalMap docMapping)
        {
            _doc = doc;
            _docMapping = docMapping;
        }

        internal async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();
            _clientMapping = await _clientMapRepository.GetByErpAsync(_doc.ClientId);
            if (_clientMapping == null)
            {
                why.Append($"Маппинг клиента id:{_doc.ClientId} не найден\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        internal async Task Create(Guid messageId)
        {
            var insertQuery = @"insert into [dbo].[FreeDoc]
                                       ([дата],[ден],[Прим],[sozdal],[dsozd],[balOK],[balOKuser],[klientID])
                                       values (@Date,@Amount,@Description,@Username,@Date,1,@Username,@ClientId);
                                       select cast(SCOPE_IDENTITY() as int)";
            var newDocId = (await _db.QueryAsync<int>(insertQuery, new
            {
                Date = _doc.Date,
                Amount = _doc.Amount,
                Description = _doc.Description,
                Username = _doc.Username,
                ClientId = _clientMapping.LegacyId
            })).FirstOrDefault();

            await _freeDocumentMapRepository.SaveAsync(new ExternalMap(messageId, newDocId, _doc.Id));
        }

        internal async Task Update()
        {
            var updateQuery = @"update [dbo].[FreeDoc] set
                                       [дата]=@Date,[ден]=@Amount,[Прим]=@Description,[klientID]=@ClientId
                                       where [код]=@Id";
            await _db.ExecuteAsync(updateQuery, new
            {
                Id = _docMapping.LegacyId,
                Date = _doc.Date,
                Amount = _doc.Amount,
                Description = _doc.Description,
                ClientId = _clientMapping.LegacyId
            });
        }
    }
}
