using Dapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LegacySql.Queries.ClientOrders.GetNotPaidClientOrderIds
{
    public class GetNotPaidClientOrderIdsQueryHandler : IRequestHandler<GetNotPaidClientOrderIdsQuery, IEnumerable<int>>
    {
        private readonly IDbConnection _db;

        public GetNotPaidClientOrderIdsQueryHandler(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<int>> Handle(GetNotPaidClientOrderIdsQuery request, CancellationToken cancellationToken)
        {
            var selectClientOrdersWithBillQuery = @"select rn.НомерПН from [dbo].[РН] rn
                                                    right join [dbo].[connected_documents] cd 
                                                    on (type1=1 and type2=10 and doc1ID in (rn.НомерПН))
                                                    right join [dbo].[Счет] bill on Код_счета = cd.doc2ID
                                                    where rn.klientID=@ClientId and (bill.оплачен is null or bill.оплачен = 0)";
            var clientOrdersWithBill = await _db.QueryAsync<int>(selectClientOrdersWithBillQuery, new
            {
                ClientId = request.ClientId
            }, commandTimeout: 300);

            return clientOrdersWithBill.Distinct();
        }
    }
}
