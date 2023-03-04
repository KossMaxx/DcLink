using System;
using System.Collections.Generic;
using MediatR;

namespace LegacySql.Queries.Diagnostics.GetProductsWithoutMapping
{
    public class GetProductsWithoutMappingQuery : IRequest<IEnumerable<Guid>>
    {
        public GetProductsWithoutMappingQuery(IEnumerable<Guid> data)
        {
            Data = data;
        }

        public IEnumerable<Guid> Data { get; }
    }
}
