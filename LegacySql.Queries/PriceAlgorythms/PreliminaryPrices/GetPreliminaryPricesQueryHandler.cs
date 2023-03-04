using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Queries.PriceAlgorythms.PreliminaryPrices
{
    public class GetPreliminaryPricesQueryHandler : IRequestHandler<GetPreliminaryPricesQuery, IEnumerable<PreliminaryPriceAlgorythmDto>>
    {
        private readonly IDbConnection _legacySqlConnection;
        private readonly AppDbContext _appDb;

        public GetPreliminaryPricesQueryHandler(IDbConnection legacySqlConnection, AppDbContext appDb)
        {
            _legacySqlConnection = legacySqlConnection;
            _appDb = appDb;
        }

        public async Task<IEnumerable<PreliminaryPriceAlgorythmDto>> Handle(GetPreliminaryPricesQuery query, CancellationToken cancellationToken)
        {
            var result = (await _legacySqlConnection.QueryAsync("PriceAlgoritmSetPrices_Data", new {PriceAlgoritmID = query.Id},
                commandType: CommandType.StoredProcedure)).ToList();

            return result.Select(async i =>
            {
                var dto = new PreliminaryPriceAlgorythmDto
                {
                    ProductId = i.tovID,
                    PriceAlgorythmId = i.PriceAlgoritmID,
                    Mark = i.Марка,
                    Ss = i.SS,
                    PriceUahMin = i.priceUAHmin,
                    PriceUahRdp = i.priceUAH_RDP,
                    PriceRrp = i.priceRRP,
                    KonkurentOpt = i.konkurent_opt,
                    KonkurentRozn = i.konkurent_rozn,
                    RateT = i.rateT,
                    Opt4X = i.opt4x, Opt4A = i.opt4a, Opt4Type = i.opt4type, Opt4Competitor = i.opt4competitor, Opt4End = i.opt4end, Price4 = i.Цена4, Opt4EndX = i.opt4endX,
                    Opt5X = i.opt5x, Opt5A = i.opt5a, Opt5Type = i.opt5type, Opt5Competitor = i.opt5competitor, Opt5End = i.opt5end, Price5 = i.Цена5, Opt5EndX = i.opt5endX,
                    Opt0X = i.opt0x, Opt0A = i.opt0a, Opt0Type = i.opt0type, Opt0Competitor = i.opt0competitor, Opt0End = i.opt0end, Price0 = i.Цена0, Opt0EndX = i.opt0endX,
                    Opt1X = i.opt1x, Opt1A = i.opt1a, Opt1Type = i.opt1type, Opt1Competitor = i.opt1competitor, Opt1End = i.opt1end, Price1 = i.Цена1, Opt1EndX = i.opt1endX,
                    OptIX = i.optIx, OptIA = i.optIa, OptIType = i.optItype, OptICompetitor = i.optIcompetitor, OptIEnd = i.optIend, PriceI = i.ЦенаИ, OptIEndX = i.optIendX,
                    MinRent = i.minRent,
                    Nal = i.nal,
                    ActiveRrp = i.ActiveRRP,
                    Price3 = i.Цена3,
                    PriceUah = i.priceUAH, PriceUahX = i.priceUAHx, PriceUahA = i.priceUAHa, PriceUahType = i.priceUAHtype, PriceUahCompetitor = i.priceUAHcompetitor, PriceUahEnd = i.priceUAHend, PriceUahEndX = i.priceUAHendX,
                    RrpX = i.rrpX,
                    RrpA = i.rrpA,
                    KonkurentOpt1 = i.konkurent_opt1,
                    ManualRrp = i.manual_rrp,
                    KonkurentOpt1Rozn = i.konkurent_opt1rozn,
                };

                var productMapping = await _appDb.ProductMaps.AsNoTracking()
                    .FirstOrDefaultAsync(m => m.LegacyId == dto.ProductId);

                dto.ProductGuid = productMapping?.ErpGuid;

                return dto;
            }).Select(t => t.Result).ToList();
        }
    }
}