using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;

namespace LegacySql.Commands.PriceAlgorythm.RecalculatePrices
{
    public class RecalculatePricesCommandHandler : IRequestHandler<RecalculatePricesCommand>
    {
        private readonly IDbConnection _db;

        public RecalculatePricesCommandHandler(IDbConnection db)
        {
            _db = db;
        }

        public async Task<Unit> Handle(RecalculatePricesCommand command, CancellationToken cancellationToken)
        {
            await _db.QueryAsync("PriceAlgoritmSetPrices", new {PriceAlgoritmID = command.Id}, commandType: CommandType.StoredProcedure);
            
            return new Unit();
        }
    }
}