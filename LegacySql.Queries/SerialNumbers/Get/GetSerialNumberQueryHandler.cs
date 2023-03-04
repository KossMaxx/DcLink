using Dapper;
using LegacySql.Data;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Products;
using LegacySql.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Queries.SerialNumbers
{
    public class GetSerialNumberQueryHandler : IRequestHandler<GetSerialNumberQuery, SerialNumberDto>
    {
        private readonly IDbConnection _db;
        private readonly AppDbContext _mapDb;
        private readonly IProductMappingResolver _productMappingResolver;
        private readonly IClientMapRepository _clientMapRepository;

        public GetSerialNumberQueryHandler(
            IDbConnection db, 
            AppDbContext mapDb, 
            IProductMappingResolver productMappingResolver, 
            IClientMapRepository clientMapRepository)
        {
            _db = db;
            _mapDb = mapDb;
            _productMappingResolver = productMappingResolver;
            _clientMapRepository = clientMapRepository;
        }

        public async Task<SerialNumberDto> Handle(GetSerialNumberQuery request, CancellationToken cancellationToken)
        {
            ExternalMap productMapping = null;
            if (request.ClientId.HasValue)
            {
                productMapping = await _clientMapRepository.GetByErpAsync(request.ClientId.Value);
                if (productMapping == null)
                {
                    return new SerialNumberDto();
                }
            }

            var result = new SerialNumberDto
            {
                Sales = await GetSales(request.SerialNumber, productMapping?.LegacyId, cancellationToken),
                Purchases = await GetPurcahses(request.SerialNumber, productMapping?.LegacyId, cancellationToken)
            };

            return result;
        }

        private async Task<IEnumerable<SerialNumberDto.SerialNumberPurchaseDto>> GetPurcahses(string serialNumber, int? clientId, CancellationToken cancellationToken)
        {
            var sqlPurchases = new StringBuilder(@"select ПН.Дата as Date, ПН.НомерПН as Number, Приход.КодТовара as ProductSqlId, ПН.klientID as ClientSqlId, Приход.Цена as ProductPrice
                                from dbo.Приход
                                inner join dbo.ПН on Приход.НомерПН = ПН.НомерПН
                                inner join dbo.sernom on Приход.КодОперации = sernom.posID                     
                                where ПН.Тип <> 3 and sernom.серном = @SerialNumber");
            if (clientId.HasValue)
            {
                sqlPurchases.Append(" and ПН.klientID = @ClientId");
            }

            var resultPurchases = await _db.QueryAsync<SerialNumberDto.SerialNumberPurchaseDto>(sqlPurchases.ToString(), new { SerialNumber = serialNumber, ClientId = clientId });
            var purchases = new List<SerialNumberDto.SerialNumberPurchaseDto>();
            foreach (var purchase in resultPurchases)
            {
                purchases.Add(new SerialNumberDto.SerialNumberPurchaseDto
                {
                    Date = purchase.Date,
                    Number = purchase.Number,
                    ClientSqlId = purchase.ClientSqlId,
                    ClientId = (await _mapDb.ClientMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == purchase.ClientSqlId, cancellationToken))?.ErpGuid,
                    ProductSqlId = purchase.ProductSqlId,
                    ProductId = (await _productMappingResolver.ResolveMappingAsync(purchase.ProductSqlId, null, cancellationToken)).productErpGuid,
                    ProductPrice = purchase.ProductPrice
                });
            }

            return purchases;
        }

        private async Task<IEnumerable<SerialNumberDto.SerialNumberSaleDto>> GetSales(string serialNumber, int? clientId, CancellationToken cancellationToken)
        {
            var sqlSales = new StringBuilder(@"select РН.DataSozd as Date, РН.НомерПН as Number, Расход.КодТовара as ProductSqlId, РН.klientID as ClientSqlId, Расход.Цена as ProductPrice
                            from rushod 
                            inner join Расход on Расход.КодОперации = rushod.Num 
                            inner join РН on РН.НомерПН = Расход.НомерПН
                            where rushod.Snom = @SerialNumber");
            if (clientId.HasValue)
            {
                sqlSales.Append(" and РН.klientID = @ClientId");
            }

            var resultSales = await _db.QueryAsync<SerialNumberDto.SerialNumberSaleDto>(sqlSales.ToString(), new { SerialNumber = serialNumber, ClientId = clientId });
            var sales = new List<SerialNumberDto.SerialNumberSaleDto>();
            foreach (var sale in resultSales)
            {
                sales.Add(new SerialNumberDto.SerialNumberSaleDto
                {
                    Date = sale.Date,
                    Number = sale.Number,
                    ClientSqlId = sale.ClientSqlId,
                    ClientId = (await _mapDb.ClientMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == sale.ClientSqlId, cancellationToken))?.ErpGuid,
                    ProductSqlId = sale.ProductSqlId,
                    ProductId = (await _productMappingResolver.ResolveMappingAsync(sale.ProductSqlId, null, cancellationToken)).productErpGuid,
                    ProductPrice = sale.ProductPrice
                });
            }

            return sales;
        }
    }
}
