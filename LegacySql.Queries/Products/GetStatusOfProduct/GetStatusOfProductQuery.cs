using System;
using MediatR;

namespace LegacySql.Queries.Products.GetStatusOfProduct
{
    public class GetStatusOfProductQuery : IRequest<ProductStatusDto>
    {
        public int ProductId { get; }
        public Guid ErpGuid { get; }

        public GetStatusOfProductQuery(int productId, Guid erpGuid)
        {
            ProductId = productId;
            ErpGuid = erpGuid;
        }
    }
}
