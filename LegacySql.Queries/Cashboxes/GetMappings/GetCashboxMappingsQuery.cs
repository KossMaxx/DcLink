using System.Collections.Generic;
using LegacySql.Queries.Shared;
using MediatR;

namespace LegacySql.Queries.Cashboxes.GetMappings
{
    public class GetCashboxMappingsQuery : IRequest<IEnumerable<CashboxMappingDto>> { }
}