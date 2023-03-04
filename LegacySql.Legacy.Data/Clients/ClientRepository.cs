using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Domain.Clients;
using Dapper;
using System.Data;

namespace LegacySql.Legacy.Data.Clients
{
    public class ClientRepository : ILegacyClientRepository
    {
        private readonly AppDbContext _mapDb;
        private readonly LegacyDbConnection _sqlConnection;

        public ClientRepository(AppDbContext mapDb, LegacyDbConnection sqlConnection)
        {
            _mapDb = mapDb;
            _sqlConnection = sqlConnection;
        }

        public async Task<Client> GetClient(int id, CancellationToken cancellationToken)
        {
            var clientIdsList = new List<int> { id };
            var clientData = await GetClientsFromDb(null, clientIdsList);
            var clientProductsGroupsData = await GetClientsProductGroupsFromDb(clientIdsList);
            var clientActivityTypesData = await GetClientsActivityTypesFromDb(clientIdsList);

            var maps = await Maps.Create(clientData, clientProductsGroupsData, clientActivityTypesData, _mapDb, cancellationToken);
            var clientMapper = new ClientMapper(
                maps.ClientMap, 
                maps.ManagerMap, 
                maps.MarketSegmentMap, 
                maps.WarehouseMap, 
                maps.DepartmentMap, 
                maps.ProductGroupMap,
                maps.ActivityTyepMap,
                maps.SegmentationTurnoverMap);

            var masterBalance = clientData.First(c => c.Client_MasterId == id);

            var client = clientMapper.Map(
                masterBalance, 
                clientData.Where(c => c.Client_Id == id).ToList(), 
                clientProductsGroupsData.Where(e=>e.ClientSegmentationProductGroups_ClientMasterId == id).ToList(),
                clientActivityTypesData.Where(e => e.ClientSegmentationActivityType_ClientMasterId == id).ToList());

            var balances = clientData.Where(c => c.Client_Id != id)
            .GroupBy(c => c.Client_Id)
            .Select(balanceGroup =>
            {
                var balance = balanceGroup.First();
                var client = clientMapper.Map(balance, balanceGroup.Select(c => c).ToList());
                return client;
            }).ToList();

            client.Nested = balances;

            return client;
        }

        public async Task<(IEnumerable<Client> clients, DateTime? lastDate)> GetChangedClients(DateTime? changedAt, CancellationToken cancellationToken, IEnumerable<int> notFullMappedIds = null)
        {
            var clientsData = await GetClientsFromDb(changedAt, notFullMappedIds);
            var clientProductsGroupsData = await GetClientsProductGroupsFromDb();
            var clientActivityTypesData = await GetClientsActivityTypesFromDb();
            var maps = await Maps.Create(clientsData, clientProductsGroupsData, clientActivityTypesData, _mapDb, cancellationToken);

            var clientMapper = new ClientMapper(
                maps.ClientMap, 
                maps.ManagerMap, 
                maps.MarketSegmentMap, 
                maps.WarehouseMap, 
                maps.DepartmentMap, 
                maps.ProductGroupMap,
                maps.ActivityTyepMap,
                maps.SegmentationTurnoverMap);
            var clients = clientsData.GroupBy(c => c.Client_MasterId)
                .Select(masterBalanceGroup =>
                {
                    var masterBalanceItems = masterBalanceGroup.Where(c => c.Client_Id == masterBalanceGroup.Key).ToList();
                    var masterBalance = masterBalanceItems.First();
                    var client = clientMapper.Map(
                        masterBalance, 
                        masterBalanceItems,
                        clientProductsGroupsData.Where(e => e.ClientSegmentationProductGroups_ClientMasterId == masterBalanceGroup.Key).ToList(),
                        clientActivityTypesData.Where(e => e.ClientSegmentationActivityType_ClientMasterId == masterBalanceGroup.Key).ToList());

                    var balances = masterBalanceGroup.Where(c => c.Client_Id != masterBalance.Client_Id)
                    .GroupBy(c => c.Client_Id)
                    .Select(balanceGroup =>
                    {
                        var balance = balanceGroup.First();
                        var client = clientMapper.Map(balance, balanceGroup.Select(c => c).ToList());

                        return client;
                    }).ToList();

                    client.Nested = balances;

                    return client;
                }).ToList();

            var lastChangeDate = clientsData.Select(c =>
                new List<DateTime?>
                {
                    c.Client_ChangedAt,
                    c.Firm?.Firms_LastChangeDate
                }.Max()
            ).Max();

            return (clients, lastChangeDate);
        }

        private async Task<IEnumerable<ClientData>> GetClientsFromDb(DateTime? changedAt = null, IEnumerable<int> idsFilter = null)
        {
            var procedure = "dbo.E21_pkg_get_clients_changed_by_checkdate";

            var procedureParams = new
            {
                check_date = changedAt,
                Client_MasterId_list = idsFilter == null || !idsFilter.Any() ? null : string.Join(",", idsFilter)
            };

            var clientsData = await _sqlConnection.Connection.QueryAsync<ClientData, FirmData, DeliveryAddressData, WarehousePriorityData, WarehouseAccessData, ClientData>(
                sql: procedure,
                param: procedureParams,
                map: (client, firm, address, warehousePriority, warehouseAccess) =>
                {
                    client.Firm = firm?.Firms_Id == 0 ? null : firm;
                    client.Address = address?.ClientDeliveryAddress_Id == 0 ? null : address;
                    client.WarehousePriority = warehousePriority?.ClientWarehousePriority_WarehouseId == 0 ? null : warehousePriority;
                    client.WarehouseAccess = warehouseAccess?.ClientWarehouseAccess_WarehouseId == 0 ? null : warehouseAccess;
                    return client;
                },
                splitOn: "Firms_row_id, ClientDeliveryAddress_Id, ClientWarehousePriority_Id, ClientWarehouseAccess_Id",
                commandTimeout: 60,
                commandType: CommandType.StoredProcedure);

            return clientsData;
        }

        private async Task<IEnumerable<ClientProductsGroupsData>> GetClientsProductGroupsFromDb(IEnumerable<int> idsFilter = null)
        {
            var procedure = "dbo.E21_pkg_get_clients_product_group";

            var procedureParams = new
            {
                Client_MasterId_list = idsFilter == null || !idsFilter.Any() ? null : string.Join(",", idsFilter)
            };

            var clientsProductGroupsData = await _sqlConnection.Connection.QueryAsync<ClientProductsGroupsData>(
                sql: procedure,
                param: procedureParams,
                commandTimeout: 60,
                commandType: CommandType.StoredProcedure);

            return clientsProductGroupsData;
        }

        private async Task<IEnumerable<ClientActivityTypesData>> GetClientsActivityTypesFromDb(IEnumerable<int> idsFilter = null)
        {
            var procedure = "dbo.E21_pkg_get_clients_activity_type";

            var procedureParams = new
            {
                Client_MasterId_list = idsFilter == null || !idsFilter.Any() ? null : string.Join(",", idsFilter)
            };

            var clientsActivityTypesData = await _sqlConnection.Connection.QueryAsync<ClientActivityTypesData>(
                sql: procedure,
                param: procedureParams,
                commandTimeout: 60,
                commandType: CommandType.StoredProcedure);

            return clientsActivityTypesData;
        }

        private class Maps
        {
            public IDictionary<int, Guid?> ClientMap { get; set; }
            public IDictionary<int, Guid?> ManagerMap { get; set; }
            public IDictionary<int, Guid?> MarketSegmentMap { get; set; }
            public IDictionary<int, Guid?> WarehouseMap { get; set; }
            public IDictionary<int, Guid?> DepartmentMap { get; set; }
            public IDictionary<int, Guid?> ProductGroupMap { get; set; }
            public IDictionary<int, Guid?> ActivityTyepMap { get; set; }
            public IDictionary<int, Guid?> SegmentationTurnoverMap { get; set; }

            public static async Task<Maps> Create(
                IEnumerable<ClientData> data, 
                IEnumerable<ClientProductsGroupsData> productGrousData, 
                IEnumerable<ClientActivityTypesData> activityTypesData, 
                AppDbContext mapDb, 
                CancellationToken cancellationToken)
            {
                var uniqClientIds = new List<int>();
                var uniqManagerIds = new List<int>();
                var uniqMarketSegmentsIds = new List<int>();
                var uniqWarehouseIds = new List<int>();
                var uniqDepartmentIds = new List<int>();
                var uniqProductGroupIds = new List<int>();
                var uniqActivityTypeIds = new List<int>();
                var uniqSegmentationTurnoverIds = new List<int>();

                foreach (var clientDataItem in data)
                {
                    uniqClientIds.Add(clientDataItem.Client_Id);

                    if (clientDataItem.Client_MainManagerId.HasValue)
                    {
                        uniqManagerIds.Add(clientDataItem.Client_MainManagerId.Value);
                    }
                    if (clientDataItem.Client_ResponsibleManagerId.HasValue)
                    {
                        uniqManagerIds.Add(clientDataItem.Client_ResponsibleManagerId.Value);
                    }

                    if (clientDataItem.Client_MarketSegmentId.HasValue)
                    {
                        uniqMarketSegmentsIds.Add(clientDataItem.Client_MarketSegmentId.Value);
                    }

                    if (clientDataItem.WarehouseAccess != null)
                    {
                        uniqWarehouseIds.Add(clientDataItem.WarehouseAccess.ClientWarehouseAccess_WarehouseId);
                    }
                    if (clientDataItem.WarehousePriority != null)
                    {
                        uniqWarehouseIds.Add(clientDataItem.WarehousePriority.ClientWarehousePriority_WarehouseId);
                    }

                    if (clientDataItem.Client_Department.HasValue)
                    {
                        uniqDepartmentIds.Add(clientDataItem.Client_Department.Value);
                    }

                    if (clientDataItem.Client_MarketSegmentationTurnover.HasValue)
                    {
                        uniqSegmentationTurnoverIds.Add(clientDataItem.Client_MarketSegmentationTurnover.Value);
                    }
                }
                uniqClientIds = uniqClientIds.Distinct().ToList();
                uniqManagerIds = uniqManagerIds.Distinct().ToList();
                uniqMarketSegmentsIds = uniqMarketSegmentsIds.Distinct().ToList();
                uniqWarehouseIds = uniqWarehouseIds.Distinct().ToList();
                uniqDepartmentIds = uniqDepartmentIds.Distinct().ToList();
                uniqSegmentationTurnoverIds = uniqSegmentationTurnoverIds.Distinct().ToList();

                var clientMap = await mapDb.ClientMaps.AsNoTracking()
                    .Where(cm => uniqClientIds.Contains(cm.LegacyId))
                    .ToDictionaryAsync(cm => cm.LegacyId, cm => cm.ErpGuid, cancellationToken);

                var managerMap = await mapDb.EmployeeMaps.AsNoTracking()
                    .Where(mm => uniqManagerIds.Contains(mm.LegacyId))
                    .ToDictionaryAsync(mm => mm.LegacyId, mm => mm.ErpGuid, cancellationToken);

                var marketSegmentMap = await mapDb.MarketSegmentMaps.AsNoTracking()
                    .Where(msm => uniqMarketSegmentsIds.Contains(msm.LegacyId))
                    .ToDictionaryAsync(msm => msm.LegacyId, msm => msm.ErpGuid, cancellationToken);

                var warehouseMap = await mapDb.WarehouseMaps.AsNoTracking()
                    .Where(wm => uniqWarehouseIds.Contains(wm.LegacyId))
                    .ToDictionaryAsync(wm => wm.LegacyId, wm => wm.ErpGuid, cancellationToken);

                var departmentMap = await mapDb.DepartmentMaps.AsNoTracking()
                    .Where(dm => uniqDepartmentIds.Contains(dm.LegacyId))
                    .ToDictionaryAsync(dm => dm.LegacyId, dm => dm.ErpGuid, cancellationToken);

                var segmentationTurnoverMap = await mapDb.SegmentationTurnoverMaps.AsNoTracking()
                    .Where(dm => uniqSegmentationTurnoverIds.Contains(dm.LegacyId))
                    .ToDictionaryAsync(dm => dm.LegacyId, dm => dm.ErpGuid, cancellationToken);

                foreach (var productGroup in productGrousData)
                {
                    uniqProductGroupIds.Add(productGroup.SegmentationProductGroups_Id);
                }
                uniqProductGroupIds = uniqProductGroupIds.Distinct().ToList();
                var productGroupMap = await mapDb.PartnerProductGroupMaps.AsNoTracking()
                    .Where(pg => uniqProductGroupIds.Contains(pg.LegacyId))
                    .ToDictionaryAsync(pg => pg.LegacyId, dm => dm.ErpGuid, cancellationToken);

                foreach (var activityType in activityTypesData)
                {
                    uniqActivityTypeIds.Add(activityType.SegmentationActivityType_Id);
                }
                uniqActivityTypeIds = uniqActivityTypeIds.Distinct().ToList();
                var activityTypeMap = await mapDb.ActivityTypes.AsNoTracking()
                    .Where(pg => uniqActivityTypeIds.Contains(pg.LegacyId))
                    .ToDictionaryAsync(pg => pg.LegacyId, dm => dm.ErpGuid, cancellationToken);

                return new Maps
                {
                    ClientMap = clientMap,
                    ManagerMap = managerMap,
                    DepartmentMap = departmentMap,
                    MarketSegmentMap = marketSegmentMap,
                    WarehouseMap = warehouseMap,
                    ProductGroupMap = productGroupMap,
                    ActivityTyepMap = activityTypeMap,
                    SegmentationTurnoverMap = segmentationTurnoverMap
                };
            }
        }
    }
}

