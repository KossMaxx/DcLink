using Dapper;
using LegacySql.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Queries.PriceLists.GetUnregistered
{
    public class GetUnregisteredProductPricesQueryHandler : IRequestHandler<GetUnregisteredProductPricesQuery, IEnumerable<UnregisteredProductPriceDto>>
    {
        private readonly IDbConnection _legacySqlConnection;
        private readonly AppDbContext _appDb;

        public GetUnregisteredProductPricesQueryHandler(IDbConnection legacySqlConnection, AppDbContext appDb)
        {
            _legacySqlConnection = legacySqlConnection;
            _appDb = appDb;
        }

        public async Task<IEnumerable<UnregisteredProductPriceDto>> Handle(GetUnregisteredProductPricesQuery request,
            CancellationToken cancellationToken)
        {
            if (!IsValid(request))
            {
                throw new ArgumentException("Хотя бы одно из значений должно быть заполнено");
            }

            var queryParams = new Params
            {
                VendorCode = request.VendorCode,
                ClientTitle = request.ClientTitle
            };

            var execSqlQuery = GetSqlQuery(queryParams);
            
            var sqlResult = await _legacySqlConnection.QueryAsync<UnregisteredProductPrice>(execSqlQuery, queryParams);

            return sqlResult.Select(e => new UnregisteredProductPriceDto
            {
                ImportDate = e.ImportDate,
                CounterPartyPrice = e.CounterPartyPrice,
                PosCode = e.PosCode,
                GoodsId = e.GoodsId,
                GoodsCategory = e.GoodsCategory,
                Manufacturer = e.Manufacturer,
                VendorCode = e.VendorCode,
                GoodsName = e.GoodsName,
                Warranty = e.Warranty,
                Weight = e.Weight,
                Volume = e.Volume,
                Width = e.Width,
                Height = e.Height,
                Depth = e.Depth,
                EAN = e.EAN,
                ZEDCode = e.ZEDCode,
                RegistarionCountry = e.RegistarionCountry,
                OriginCountry = e.OriginCountry,
                givenCostOfPrice = e.givenCostOfPrice
            });
        }

        private string GetSqlQuery(Params queryParams)
        {
            var sqlQuery = new StringBuilder(@"WITH ReportPrice AS(SELECT XML.II AS PriceId,
                                                   K.Название AS CounterPartyPrice,
                                                   COALESCE(XML.price1, XML.price_UAH * R.ratec) AS givenCostOfPrice
                                               FROM [dbo].XMLprice XML
                                                   LEFT JOIN [dbo].Клиенты K ON XML.klientID = K.КодПоставщика
                                               LEFT JOIN [dbo].RateV R ON K.ВалютаБаланса = R.rateID
                                               WHERE XML.productId IS NULL");
            if (!string.IsNullOrEmpty(queryParams.VendorCode))
            {
                sqlQuery.Append(@" and [Artikul] = @VendorCode");
            }
            if (!string.IsNullOrEmpty(queryParams.ClientTitle))
            {
                sqlQuery.Append(@" and [Название] = @ClientTitle");
            }

            sqlQuery.Append(@") SELECT T.date_last_success AS ImportDate,
                                       RP.CounterPartyPrice AS CounterPartyPrice,
                                       X.PosCode AS PosCode,
                                       X.productId AS GoodsId,
                                       X.Tip AS GoodsCategory,
                                       X.Brand AS Manufacturer,
                                       X.Artikul AS VendorCode,
                                       X.Marka AS GoodsName,
                                       X.war AS Warranty,
                                       X.weight AS Weight,
                                       X.volume AS Volume,
                                       X.width AS Width,
                                       X.height AS Height,
                                       X.depth AS Depth,
                                       X.barcode AS EAN,
                                       X.tnved AS ZEDCode,
                                       X.countryOfRegistration AS RegistarionCountry,
                                       X.countryOfOrigin AS OriginCountry,
                                       RP.givenCostOfPrice AS givenCostOfPrice
                                FROM ReportPrice RP
                                         INNER JOIN [dbo].XMLprice X ON RP.PriceId = X.II
                                         LEFT JOIN  [dbo].TBL_price_agregator T ON X.klientID = T.client_id
                                ORDER BY givenCostOfPrice ASC, date_last_success DESC, VendorCode DESC");

            return sqlQuery.ToString();
        }

        private bool IsValid(GetUnregisteredProductPricesQuery request)
        {
            return (request.ClientTitle != null || request.VendorCode != null);             
        }

        private class Params
        {
            public string VendorCode { get; set; }
            public string ClientTitle { get; set; }
        }

        private class UnregisteredProductPrice
        {
            public DateTime ImportDate { get; set; }
            public string CounterPartyPrice { get; set; }
            public string PosCode { get; set; }
            public string GoodsId { get; set; }
            public string GoodsCategory { get; set; }
            public string Manufacturer { get; set; }
            public string VendorCode { get; set; }
            public string GoodsName { get; set; }
            public string Warranty { get; set; }
            public int? Weight { get; set; }
            public int? Volume { get; set; }
            public int? Width { get; set; }
            public int? Height { get; set; }
            public int? Depth { get; set; }
            public string EAN { get; set; }
            public string ZEDCode { get; set; }
            public string RegistarionCountry { get; set; }
            public string OriginCountry { get; set; }
            public string givenCostOfPrice { get; set; }
        }
    }   
}
