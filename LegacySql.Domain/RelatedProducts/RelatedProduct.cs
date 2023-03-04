using System;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.RelatedProducts
{
    public class RelatedProduct
    {
        public int Id { get; }
        public IdMap MainProductId { get; }
        public IdMap RelatedProductId { get; }
        public DateTime? ChangedAt { get; set; }

        public RelatedProduct(int id, IdMap mainProductId, IdMap relatedProductId, DateTime? changedAt)
        {
            Id = id;
            MainProductId = mainProductId;
            RelatedProductId = relatedProductId;
            ChangedAt = changedAt;
        }

        public MappingInfo IsMappingsFull()
        {
            var why = new StringBuilder();
            var mainProductId_notEmpty = MainProductId?.ExternalId != null;
            if (!mainProductId_notEmpty)
            {
                why.Append($"Поле: MainProductId., Id: {MainProductId?.InnerId}\n");
            }
            var relatedProductId_notEmpty = RelatedProductId?.ExternalId != null;
            if (!relatedProductId_notEmpty)
            {
                why.Append($"Поле: MainProductId., Id: {RelatedProductId?.InnerId}\n");
            }

            return new MappingInfo
            {
                IsMappingFull = mainProductId_notEmpty && relatedProductId_notEmpty,
                Why = why.ToString()
            };
        }
    }
}