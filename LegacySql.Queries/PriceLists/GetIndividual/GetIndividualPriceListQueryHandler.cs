using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.PriceLists.GetIndividual
{
    public class GetIndividualPriceListQueryHandler : IRequestHandler<GetIndividualPriceListQuery, IEnumerable<IndividualPriceListItemDto>>
    {
        private readonly IDbConnection _legacySqlConnection;
        private readonly AppDbContext _appDb;

        public GetIndividualPriceListQueryHandler(IDbConnection legacySqlConnection, AppDbContext appDb)
        {
            _legacySqlConnection = legacySqlConnection;
            _appDb = appDb;
        }

        public async Task<IEnumerable<IndividualPriceListItemDto>> Handle(GetIndividualPriceListQuery request,
            CancellationToken cancellationToken)
        {
            var queryParams = new Params
            {
                ClientId = await GetClientId(request.ClientId, cancellationToken),
                CategoryId = await GetProductTypeId(request.ProductTypeId, cancellationToken),
                Manufacturer = await GetManufacturer(request.ManufacturerId, cancellationToken),
                ProductManager = await GetProductManagerLogin(request.ProductManagerId, cancellationToken)
            };
            var execSqlQuery = @"exec [dbo].[ERP_GetClientsPriceList] 
                               @client_id = @ClientId,
                               @category_id = @CategoryId, 
                               @vendor = @Manufacturer, 
                               @product_manager = @ProductManager
                               WITH RESULT SETS
                               (
                                ( 
                                 ProductId INT,
                                 VendorCode VARCHAR(100),
                                 ProductType VARCHAR(100),
                                 Subtype VARCHAR(100),
                                 Manufacturer INT,
                                 WorkName VARCHAR(100),
                                 ClientPrice MONEY,
                                 WarehouseQuantity INT,
                                 RetailPrice MONEY,
                                 Warranty INT,
                                 RRP TINYINT
                                ) 
                               )";
            var sqlResult = (await _legacySqlConnection.QueryAsync<LegacyIndividualPriceListItemRequestResult>(execSqlQuery, queryParams));

            return sqlResult.Select(async e => new IndividualPriceListItemDto
            {
                ProductId = await GetProductErpId(e.ProductId, cancellationToken),
                VendorCode = e.VendorCode,
                ProductTypeId = request.ProductTypeId ?? await GetProductTypeErpId(e.ProductType, cancellationToken),
                SubtypeId = await GetProductSubtypeErpId(e.Subtype, cancellationToken),
                ManufacturerId = await GetManufacturerErpId(e.Manufacturer, cancellationToken),
                WorkName = e.WorkName,
                ClientPrice = e.ClientPrice,
                WarehouseQuantity = e.WarehouseQuantity,
                Warranty = e.Warranty,
                RetailPrice = e.RetailPrice,
                IsMonitoring = e.IsMonitoring
            }).Select(e => e.Result);
        }

        private async Task<Guid?> GetManufacturerErpId(int? manufacturer, CancellationToken cancellationToken)
        {
            if (manufacturer == null)
            {
                return null;
            }

            var map = await _appDb.ManufacturerMaps.AsNoTracking()
                .FirstOrDefaultAsync(p => p.LegacyId == manufacturer, cancellationToken);
            
            return map?.ErpGuid;
        }

        private async Task<Guid?> GetProductSubtypeErpId(string subtype, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subtype))
            {
                return null;
            }

            var map = await _appDb.ProductSubtypeMaps.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Title == subtype, cancellationToken);
            
            return map?.ErpGuid;
        }

        private async Task<Guid?> GetProductTypeErpId(string type, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(type))
            {
                return null;
            }

            var selectSqlQuery = @"select [КодТипа] from [dbo].[Типы]
                                 where [НазваниеТипа]=@Title";
            var productTypeLegacyId = await _legacySqlConnection.QueryFirstOrDefaultAsync<int?>(selectSqlQuery, new
            {
                Title = type
            });

            if (!productTypeLegacyId.HasValue)
            {
                return null;
            }

            var map = await _appDb.ProductTypeMaps.FirstOrDefaultAsync(p => p.LegacyId == productTypeLegacyId, cancellationToken);
            
            return map?.ErpGuid;
        }

        private async Task<Guid?> GetProductErpId(long id, CancellationToken cancellationToken)
        {
            var map = await _appDb.ProductMaps.AsNoTracking()
                .FirstOrDefaultAsync(p => p.LegacyId == id, cancellationToken);

            return map?.ErpGuid;
        }

        private async Task<long> GetClientId(Guid id, CancellationToken cancellationToken)
        {
            var map = await _appDb.ClientMaps.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ErpGuid.HasValue && p.ErpGuid == id, cancellationToken);
            var clientId = map?.LegacyId;

            if (!clientId.HasValue)
            {
                throw new ArgumentException($"Маппинг клиента продукта id:{id} не найден\n");
            }

            return clientId.Value;
        }

        private async Task<int?> GetManufacturer(Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
            {
                return null;
            }

            var map = await _appDb.ManufacturerMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ErpGuid == id, cancellationToken);
            var manufacturer = map?.LegacyId;

            if (manufacturer == null)
            {
                throw new ArgumentException($"Маппинг производителя id:{id} не найден\n");
            }

            return manufacturer;
        }

        private async Task<long?> GetProductTypeId(Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
            {
                return null;
            }

            var map = await _appDb.ProductTypeMaps.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ErpGuid.HasValue && p.ErpGuid == id, cancellationToken);
            var productTypeId = map?.LegacyId;

            if (!productTypeId.HasValue)
            {
                throw new ArgumentException($"Маппинг типа продукта id:{id} не найден\n");
            }

            return productTypeId;
        }

        private async Task<string> GetProductManagerLogin(Guid? id, CancellationToken cancellationToken)
        {
            if (!id.HasValue)
            {
                return null;
            }

            var map = await _appDb.EmployeeMaps.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ErpGuid == id, cancellationToken);
            if (map == null)
            {
                throw new ArgumentException($"Маппинг сотрудника id:{id} не найден\n");
            }

            var sql = @"SELECT uuu
                        FROM dbo.Сотрудники
                        WHERE КодСотрудника = @Id";
            var productManager = await _legacySqlConnection.QueryFirstOrDefaultAsync<string>(sql, new
            {
                Id = map.LegacyId,
            });

            return productManager;
        }

        private class Params
        {
            public long ClientId { get; set; }
            public long? CategoryId { get; set; }
            public int? Manufacturer { get; set; }
            public string ProductManager { get; set; }
        }

        private class LegacyIndividualPriceListItemRequestResult
        {
            public long ProductId { get; set; }
            public string VendorCode { get; set; }
            public string Subtype { get; set; }
            public string ProductType { get; set; }
            public int Manufacturer { get; set; }
            public string WorkName { get; set; }
            public decimal ClientPrice { get; set; }
            public int WarehouseQuantity { get; set; }
            public int Warranty { get; set; }
            public decimal? RetailPrice { get; set; }
            public byte RRP { get; set; }
            public bool IsMonitoring => RRP == 1;
        }
    }
}
