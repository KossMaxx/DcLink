using System;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.PriceConditions
{
    public class PriceCondition
    {
        public IdMap Id { get; }
        public DateTime? Date { get; }
        public IdMap ClientId { get; }
        public IdMap ProductTypeId { get; }
        public IdMap VendorId { get; }
        public string ProductManager { get; }
        public short? PriceType { get; }
        public DateTime? DateTo { get; }
        public string Comment { get; }
        public decimal? Value { get; }
        public decimal? PercentValue { get; }
        public int? UpperThresholdPriceType { get; }

        public PriceCondition(IdMap id, DateTime? date, IdMap clientId, IdMap productTypeId, IdMap vendorId, string productManager, short? priceType, DateTime? dateTo, string comment, decimal? value, decimal? percentValue, int? upperThresholdPriceType)
        {
            Id = id;
            Date = date;
            ClientId = clientId;
            ProductTypeId = productTypeId;
            VendorId = vendorId;
            ProductManager = productManager;
            PriceType = priceType;
            DateTo = dateTo;
            Comment = comment;
            Value = value;
            PercentValue = percentValue;
            UpperThresholdPriceType = upperThresholdPriceType;
        }

        public MappingInfo IsMappingsFull()
        {
            var why = new StringBuilder();
            
            if (ClientId != null && !ClientId.ExternalId.HasValue)
            {
                why.Append($"Поле: ClientId., Id: {ClientId?.InnerId}\n");
            }
            
            if (ProductTypeId != null && !ProductTypeId.ExternalId.HasValue)
            {
                why.Append($"Поле: ProductTypeId., Id: {ProductTypeId?.InnerId}\n");
            }

            if (VendorId != null && !VendorId.ExternalId.HasValue)
            {
                why.Append($"Поле: ManufactureId. Id: {VendorId.InnerId}\n");
            }

            var whyResult = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyResult),
                Why = whyResult,
            };
        }
    }
}
