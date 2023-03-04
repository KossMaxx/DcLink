using System;
using MediatR;

namespace LegacySql.Queries.Products.IsProductMappingExist
{
    public class IsProductMappingExistQuery : IRequest<bool>
    {
        public IsProductMappingExistQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
