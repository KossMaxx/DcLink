using Dapper;
using LegacySql.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Manufacturer;

namespace LegacySql.Queries.PriceAlgorythms.Get
{
    public class GetPriceAlgorythmQueryHandler : IRequestHandler<GetPriceAlgorythmQuery, PriceAlgorythmDto>
    {
        private readonly IDbConnection _legacySqlConnection;
        private readonly AppDbContext _appDb;

        public GetPriceAlgorythmQueryHandler(IDbConnection legacySqlConnection, AppDbContext appDb)
        {
            _legacySqlConnection = legacySqlConnection;
            _appDb = appDb;
        }

        public async Task<PriceAlgorythmDto> Handle(GetPriceAlgorythmQuery query, CancellationToken cancellationToken)
        {
            var sql = @"select 
							PriceAlgoritmID as Id, 
							Nazvanie as Title,
							PriceAlgoritm.Manager as ManagerLogin,
							Сотрудники.КодСотрудника as ManagerId,
							Active as Active,
							zakazonly as ZakazOnly,
							ActiveRRP as ActiveRrp,
							PriceAlgoritm.klientID as ClientId,
							ActivePriceUAH as ActivePriceUah,
							zakazonlyRRP as ZakazOnlyRrp,
							minRent as MinRent,
							rrpX as RrpX,
							rrpA as RrpA,
							rrpC as RrpC,
							algoritm_nal_type as AlgorithmNalType,
							minRent_use_sklad as MinRentUseSklad,
							DoNonDeleteRRP as DoNonDeleteRrp,	
							opt4x as Opt4X, opt4a as Opt4A, opt4type as Opt4Type, opt4competitor as Opt4Competitor, opt4end as Opt4End, opt4endX as Opt4EndX,
							opt5x as Opt5X, opt5a as Opt5A, opt5type as Opt5Type, opt5competitor as Opt5Competitor, opt5end as Opt5End, opt5endX as Opt5EndX,
							opt0x as Opt0X, opt0a as Opt0A, opt0type as Opt0Type, opt0competitor as Opt0Competitor, opt0end as Opt0End, opt0endX as Opt0EndX,
							opt1x as Opt1X, opt1a as Opt1A, opt1type as Opt1Type, opt1competitor as Opt1Competitor, opt1end as Opt1End, opt1endX as Opt1EndX,
							optIx as OptIX,	optIa as OptIA,	optItype as OptIType, optIcompetitor as OptICompetitor,	optIend as OptIEnd,	optIendX as OptIEndX,
							priceUAHx as PriceUahX, priceUAHa as PriceUahA, priceUAHtype as PriceUahType, priceUAHcompetitor as PriceUahCompetitor, priceUAHend as PriceUahEnd, priceUAHendX as PriceUahEndX
                        from PriceAlgoritm
						left join Сотрудники on Сотрудники.uuu = PriceAlgoritm.Manager
                        where PriceAlgoritmID = @Id";

            var result = await _legacySqlConnection.QueryFirstOrDefaultAsync<PriceAlgorythmDto>(sql, new {query.Id});

            if (result.ManagerId.HasValue)
            {
                var map = await _appDb.EmployeeMaps.AsNoTracking()
                    .FirstOrDefaultAsync(m => m.LegacyId == result.ManagerId.Value, cancellationToken);
                result.ManagerGuid = map?.ErpGuid;
            }

            if (result.ClientId.HasValue)
            {
                var map = await _appDb.ClientMaps
                    .FirstOrDefaultAsync(m => m.LegacyId == result.ClientId.Value, cancellationToken);
                result.ClientGuid = map?.ErpGuid;
            }

            result.Details = await GetPriceAlgorythmDetails(query.Id, cancellationToken);
            result.Settings = await GetPriceAlgorythmSettings(query.Id, cancellationToken);

            return result;
        }

        private async Task<IEnumerable<PriceAlgorythmDetailDto>> GetPriceAlgorythmDetails(int priceAlgoritmId, CancellationToken cancellationToken)
        {
            var sql = @"select 
                             PriceAlgoritmDetailsID as Id,
                             CategoryID as ProductTypeId,
						     Vendor,            							
						     klas as Class     
                        from PriceAlgoritmDetails
                        where PriceAlgoritmID = @Id";

            var result = (await _legacySqlConnection.QueryAsync<PriceAlgorythmDetailDto>(sql, new {Id = priceAlgoritmId})).ToList();

            Dictionary<int, Guid?> productTypes = new Dictionary<int, Guid?>();
            Dictionary<string, Guid?> vendors = new Dictionary<string, Guid?>();
            
            foreach (var detail in result)
            {
                if (!productTypes.ContainsKey(detail.ProductTypeId))
                {
                    var map = await _appDb.ProductTypeMaps.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.LegacyId == detail.ProductTypeId, cancellationToken);
                    productTypes[detail.ProductTypeId] = map?.ErpGuid;
                }

                if (!vendors.ContainsKey(detail.Vendor))
                {
                    var map = await _appDb.ManufacturerMaps.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.LegacyTitle == detail.Vendor, cancellationToken);
                    vendors[detail.Vendor] = map?.ErpGuid;
                }
            }

            foreach (var detail in result)
            {
                detail.ProductTypeGuid = productTypes[detail.ProductTypeId];
                detail.VendorGuid = vendors[detail.Vendor];
            }

            return result;
        }

        private async Task<IEnumerable<PriceAlgorythmSettingDto>> GetPriceAlgorythmSettings(int priceAlgoritmId, CancellationToken cancellationToken)
        {
            var sql = @"select 
                             PriceAlgoritmSuplListID as Id,
                             klientID as ClientId,
						     rrp_s as Rrp,            							
						     rdp_s as Rdp,
						     xrate as XRate,
						     price_s as Price,
						     competitor_opt as CompetitorOpt,
						     competitor_rozn as CompetitorRozn,
						     competitor_1 as Competitor1,
						     competitor_1rozn as Competitor1Rozn
						from PriceAlgoritmSuplList
                        where PriceAlgoritmID = @Id";

            var result = (await _legacySqlConnection.QueryAsync<PriceAlgorythmSettingDto>(sql, new {Id = priceAlgoritmId})).ToList();

            Dictionary<int, Guid?> clientMaps = new Dictionary<int, Guid?>();
            
            foreach (var setting in result)
            {
                if (!clientMaps.ContainsKey(setting.ClientId))
                {
                    var map = await _appDb.ClientMaps.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.LegacyId == setting.ClientId, cancellationToken);
                    clientMaps[setting.ClientId] = map?.ErpGuid;
                }
            }

            foreach (var setting in result)
            {
                setting.ClientGuid = clientMaps[setting.ClientId];
            }
            
            return result;
        }
    }
}