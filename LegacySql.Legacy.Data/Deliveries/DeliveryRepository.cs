using Dapper;
using LegacySql.Data;
using LegacySql.Domain.Deliveries;
using LegacySql.Domain.Deliveries.PublishEntity;
using LegacySql.Domain.Shared;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Legacy.Data.Deliveries
{
    public class DeliveryRepository : ILegacyDeliveryRepository
    {
        private readonly LegacyDbConnection _db;
        private readonly AppDbContext _mapDb;
        private readonly int _notFullMappingFilterPortion;

        public DeliveryRepository(
            LegacyDbConnection connection,
            AppDbContext mapDb, 
            int notFullMappingFilterPortion)
        {
            _db = connection;
            _mapDb = mapDb;
            _notFullMappingFilterPortion = notFullMappingFilterPortion;
        }

        private async IAsyncEnumerable<Delivery> GetAllAsync(string filter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var selectSqlQuery = new StringBuilder(@"select d.id as Id
                                                     , d.zadacha as TaskDescription
                                                     , d.kuda as Address
                                                     , d.kogda as DeparturePlanDate
                                                     , d.ddd as CreateDate
                                                     , d.sozd as CreatorUsername
                                                     , d.gorod as CategoryId
                                                     , dt.Nazv as CategoryTitle
                                                     , d.vipolneno as IsCompleted
                                                     , d.vipDdd as PerformedDate
                                                     , d.CargoInvoice as CargoInvoice
                                                     , d.WayBill as ReceivedEmployeeId
                                                     , d.statusDC as StatusId
                                                     , ct.carrier_type_name as CarrierTypeName
                                                     , d.finished as IsFinished
                                                     , d.modified_at as ChangedAt
                                                from dostavka d
                                                         left outer join dostavkaTip dt
                                                                         on d.gorod = dt.DistavkaTipID
                                                         left outer join dbo.TBL_carrier_types as ct
                                                                         on ct.carrier_type_id = isnull(dt.carrier_type, 0) ");

            if (string.IsNullOrEmpty(filter))
            {
                selectSqlQuery.Append("where d.ddd >= cast('2022.05.01' as date) ");
            }
            
            if (!string.IsNullOrEmpty(filter))
            {
                selectSqlQuery.Append(filter);
            }

            IEnumerable<DeliveryData> entities;
            using (var connection = _db.Connection)
            {
                entities = await connection.QueryAsync<DeliveryData>(selectSqlQuery.ToString());
            }

            await foreach (var e in MapToDomain(entities, cancellationToken))
            {
                yield return e;
            }
        }

        public async IAsyncEnumerable<Delivery> GetChangedDeliveriesAsync(DateTime? changedAt, IEnumerable<int> notFullMappingIds, CancellationToken cancellationToken)
        {
            string filter = string.Empty;
            if (changedAt.HasValue)
            {
                filter = $@"where cast(d.modified_at as date) > cast('{changedAt.Value.ToString("yyyy.MM.dd")}' as date) 
                            or cast(d.ddd as date) > cast('{changedAt.Value.ToString("yyyy.MM.dd")}' as date) ";
            }

            await foreach (var i in GetAllAsync(filter, cancellationToken))
            {
                yield return i;
            }

            await foreach (var i in GetByNotFullMapping(notFullMappingIds, cancellationToken))
            {
                yield return i;
            }
        }

        public async Task<Delivery> GetDeliveryAsync(int id, CancellationToken cancellationToken)
        {
            string filter = $@"where d.id = {id}";
            var entity = await GetAllAsync(filter, cancellationToken).ToListAsync();
            return entity.FirstOrDefault();
        }

        private async IAsyncEnumerable<Delivery> MapToDomain(IEnumerable<DeliveryData> entities,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var e in entities)
            {
                var purchase = await MapToDomain(e, cancellationToken);
                if (purchase != null)
                {
                    yield return purchase;
                }
            }
        }

        private async Task<Delivery> MapToDomain(DeliveryData deliveryEf, CancellationToken cancellationToken)
        {
            var deliveryMapping = await _mapDb.DeliveryMaps.AsNoTracking().FirstOrDefaultAsync(e => e.LegacyId == deliveryEf.Id, cancellationToken);
            var receivedEmployeeMapping = await _mapDb.EmployeeMaps.AsNoTracking().FirstOrDefaultAsync(e=>e.LegacyId == deliveryEf.ReceivedEmployeeId, cancellationToken);

            return new Delivery(
                hasMap: deliveryMapping != null,
                id: new IdMap(deliveryEf.Id, deliveryMapping?.ErpGuid),
                createDate: deliveryEf.CreateDate,
                departurePlanDate: deliveryEf.DeparturePlanDate,
                creatorUsername: deliveryEf.CreatorUsername,
                taskDescription: deliveryEf.TaskDescription,
                address: deliveryEf.Address,
                carrierTypeName: deliveryEf.CarrierTypeName,
                cargoInvoice: deliveryEf.CargoInvoice,
                statusId: deliveryEf.StatusId,
                category: deliveryEf.CategoryId.HasValue
                ? new DeliveryCategory(deliveryEf.CategoryId.Value, deliveryEf.CategoryTitle)
                : null,
                receivedEmployeeId: new IdMap(deliveryEf.ReceivedEmployeeId, receivedEmployeeMapping?.ErpGuid),
                isCompleted: deliveryEf.IsCompleted,
                performedDate: deliveryEf.PerformedDate,
                isFinished: deliveryEf.IsFinished,
                changedAt: deliveryEf.ChangedAt,
                stickers: await GetDeliveryStickers(deliveryEf.Id, cancellationToken),
                warehouses: await GetDeliveryWarehouses(deliveryEf.Id, cancellationToken)
                );
        }

        private async Task<IEnumerable<DeliveryWarehouse>> GetDeliveryWarehouses(int deliveryId, CancellationToken cancellationToken)
        {
            var sqlQuery = @"select dostavkaID as DeliveryId
                            , RN as ClientOrderId
                            , tip as TypeId 
                            from dbo.dostavkaRN
                            where dostavkaID=@DeliveryId";

            IEnumerable<DeliveryWarehouseData> entities;
            using (var connection = _db.Connection)
            {
                entities = await connection.QueryAsync<DeliveryWarehouseData>(sqlQuery, new
                {
                    DeliveryId = deliveryId
                });
            }

            if (!entities.Any())
            {
                return new List<DeliveryWarehouse>();
            }

            var clientOrderIds = entities
                .Where(e=> e.ClientOrderId.HasValue)
                .Select(e => e.ClientOrderId.Value);

            var clientOrderMappings = await _mapDb.ClientOrderMaps
                .Where(e => clientOrderIds.Contains(e.LegacyId))
                .ToDictionaryAsync(e=>e.LegacyId, e=>e.ErpGuid);

            var warehouses = new List<DeliveryWarehouse>();
            foreach (var entity in entities)
            {
                var clientOrderMapping = entity.ClientOrderId.HasValue 
                    ? new IdMap(entity.ClientOrderId.Value, clientOrderMappings.ContainsKey(entity.ClientOrderId.Value) 
                    ? clientOrderMappings[entity.ClientOrderId.Value] 
                    : null) 
                    : null;
                warehouses.Add(new DeliveryWarehouse(clientOrderMapping, entity.TypeId));
            }

            return warehouses;
        }

        private async Task<IEnumerable<string>> GetDeliveryStickers(int deliveryId, CancellationToken cancellationToken)
        {
            var sqlQuery = @"select dostavkaID as DeliveryId, Nazv as Title from dbo.dostavkaParts
                             where dostavkaID = @DeliveryId";

            IEnumerable<DeliveryStickersData> entities;
            using(var connection = _db.Connection)
            {
                entities = await connection.QueryAsync<DeliveryStickersData>(sqlQuery, new
                {
                    DeliveryId = deliveryId
                });
            }

            if (!entities.Any())
            {
                return new List<string>();
            }

            return entities.Select(e => e.Title);
        }

        private async IAsyncEnumerable<Delivery> GetByNotFullMapping(IEnumerable<int> notFullMappingIds,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (!notFullMappingIds.Any())
            {
                yield break;
            }

            var notFullMappingCount = notFullMappingIds.Count();
            var cycleLimitation = (double)notFullMappingCount / _notFullMappingFilterPortion;
            for (var i = 0; i < Math.Ceiling(cycleLimitation); i++)
            {
                var portionIds = notFullMappingIds.Skip(i * _notFullMappingFilterPortion)
                        .Take(_notFullMappingFilterPortion);

                var filter = $@"where d.id in ({string.Join(',', portionIds)})";

                await foreach (var mappedItem in GetAllAsync(filter, cancellationToken))
                {
                    yield return mappedItem;
                }
            }
        }
    }
}
