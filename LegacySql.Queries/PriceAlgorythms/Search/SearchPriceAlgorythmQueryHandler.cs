using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Queries.PriceAlgorythms.Search
{
    public class SearchPriceAlgorythmQueryHandler : IRequestHandler<SearchPriceAlgorythmQuery, IEnumerable<PriceAlgorythmReferenceDto>>
    {
        private readonly IDbConnection _legacySqlConnection;

        public SearchPriceAlgorythmQueryHandler(IDbConnection legacySqlConnection)
        {
            _legacySqlConnection = legacySqlConnection;
        }

        public async Task<IEnumerable<PriceAlgorythmReferenceDto>> Handle(SearchPriceAlgorythmQuery query, CancellationToken cancellationToken)
        {
            var sqlSearchTerm = "";
            var sql = @"select 
                            PriceAlgoritmID as Id, 
                            Nazvanie as Title 
                        from PriceAlgoritm";

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                sql += " where Nazvanie like @SearchTerm";
                sqlSearchTerm = $"%{query.SearchTerm}%";
            }

            var result = (await _legacySqlConnection.QueryAsync<PriceAlgorythmReferenceDto>(sql, new { SearchTerm = sqlSearchTerm })).ToList();

            return result;
        }
    }
}
