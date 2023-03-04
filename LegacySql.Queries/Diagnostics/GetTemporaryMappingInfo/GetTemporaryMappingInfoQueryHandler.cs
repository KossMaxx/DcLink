using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Data;
using LegacySql.Data.Models;
using LegacySql.Domain.Shared;
using LegacySql.Queries.Infrastructure;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetTemporaryMappingInfo
{
    public class GetTemporaryMappingInfoQueryHandler : IRequestHandler<GetTemporaryMappingInfoQuery, TemporaryMappingStatisticDto>
    {
        private readonly AppDbContext _mapDb;

        public GetTemporaryMappingInfoQueryHandler(AppDbContext mapDb)
        {
            _mapDb = mapDb;
        }

        public async Task<TemporaryMappingStatisticDto> Handle(GetTemporaryMappingInfoQuery request, CancellationToken cancellationToken)
        {
            switch (request.Type)
            {
                case MappingTypes.Product:
                {
                    var res = await _mapDb.ProductMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.ProductType:
                {
                    var res = await _mapDb.ProductTypeMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.Client:
                {
                    var res = await _mapDb.ClientMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.ClientOrder:
                {
                    var res = await _mapDb.ClientOrderMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.ProductTypeCategory:
                {
                    var res = await _mapDb.ProductTypeCategoryMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.ProductTypeCategoryParameter:
                {
                    var res = await _mapDb.ProductTypeCategoryParameterMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.Employee:
                {
                    var res = await _mapDb.EmployeeMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.PhysicalPerson:
                {
                    var res = await _mapDb.PhysicalPersonMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.RelatedProduct:
                {
                    var res = await _mapDb.RelatedProductMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.Purchase:
                {
                    var res = await _mapDb.PurchaseMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.Reject:
                {
                    var res = await _mapDb.RejectMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.ProductRefund:
                {
                    var res = await _mapDb.ProductRefundMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.PriceCondition:
                {
                    var res = await _mapDb.PriceConditionMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.ProductPriceCondition:
                {
                    var res = await _mapDb.ProductPriceConditionMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.MarketSegment:
                {
                    var res = await _mapDb.MarketSegmentMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.ProductSubtype:
                {
                    var res = await _mapDb.ProductSubtypeMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                case MappingTypes.Department:
                {
                    var res = await _mapDb.DepartmentMaps
                        .Where(e => !e.ErpGuid.HasValue)
                        .GetPageAsync(request.Page, request.PageSize);
                    return GetStatistic(request, res);
                }
                default: throw new ArgumentException($"Для данного типа ({request.Type.ToString()}) метод не реализован");
            }
        }

        private static TemporaryMappingStatisticDto GetStatistic(GetTemporaryMappingInfoQuery request, (IEnumerable<BaseMapModel> items, int totalItems) res)
        {
            return new TemporaryMappingStatisticDto
            {
                Page = request.Page,
                Total = res.totalItems,
                Items = res.items.Select(e => new TemporaryMappingDto
                {
                    LegacyId = e.LegacyId,
                    MapGuid = e.MapGuid
                })
            };
        }
    }
}
