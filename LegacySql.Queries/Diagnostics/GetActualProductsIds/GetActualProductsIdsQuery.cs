using System.Collections.Generic;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetActualProductsIds
{
    public class GetActualProductsIdsQuery : IRequest<IEnumerable<long>>
    {
    }
}
