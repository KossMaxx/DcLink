using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Products
{
    public class ProductCategoryParameter
    {
        public IdMap CategoryId { get; }
        public IdMap ParameterId { get; }

        public ProductCategoryParameter(IdMap categoryId, IdMap parameterId)
        {
            CategoryId = categoryId;
            ParameterId = parameterId;
        }
    }
}
