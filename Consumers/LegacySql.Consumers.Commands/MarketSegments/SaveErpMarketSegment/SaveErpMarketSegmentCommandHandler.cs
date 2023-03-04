using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.MarketSegments;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.MarketSegments.Import;

namespace LegacySql.Consumers.Commands.MarketSegments.SaveErpMarketSegment
{
    public class SaveErpMarketSegmentCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpMarketSegmentDto>>
    {
        private readonly IDbConnection _db;
        private readonly IMarketSegmentMapRepository _marketSegmentMapRepository;
        private ExternalMap _segmentMapping;

        public SaveErpMarketSegmentCommandHandler(
            IDbConnection db, 
            IMarketSegmentMapRepository marketSegmentMapRepository)
        {
            _db = db;
            _marketSegmentMapRepository = marketSegmentMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpMarketSegmentDto> command, CancellationToken cancellationToken)
        {
            var segment = command.Value;
            _segmentMapping = await _marketSegmentMapRepository.GetByErpAsync(segment.Id);
            if (_segmentMapping == null)
            {
                await Create(segment, command.MessageId);
            }
            else
            {
                await Update(segment);
            }

            return new Unit();
        }

        private async Task Update(ErpMarketSegmentDto segment)
        {
            var updateSqlQuery = @"update [dbo].[TBL_segmentation]
                                 set [name]=@Title
                                 where [id]=@Id";

            await _db.ExecuteAsync(updateSqlQuery, new
            {
                Id = _segmentMapping.LegacyId,
                Title = segment.Title
            });
        }

        private async Task Create(ErpMarketSegmentDto segment, Guid mapId)
        {
            var insertSqlQuery = @"insert into [dbo].[TBL_segmentation]
                                 ([name]) 
                                 values (@Title);
                                 select cast(SCOPE_IDENTITY() as int)";

            var newSegmentId = (await _db.QueryAsync<int>(insertSqlQuery, new
            {
                Title = segment.Title
            })).FirstOrDefault();

            await _marketSegmentMapRepository.SaveAsync(
                new ExternalMap(mapId,newSegmentId,segment.Id));
        }
    }
}
