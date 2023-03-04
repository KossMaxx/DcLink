using System;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.SupplierCurrencyRates
{
    public class SupplierCurrencyRate
    {
        public int Id { get; }
        public decimal? RateNal { get; }
        public decimal? RateBn { get; }
        public decimal? RateDdr { get; }
        public DateTime? Date { get; }
        public IdMap ClientId { get; }
        public bool ChangedByBot { get; }
        public byte? BalanceCurrencyId { get; }
        public bool? IsSupplier { get; }
        public bool? IsCustomer { get; }

        public SupplierCurrencyRate(
            int id, 
            decimal? rateNal, 
            decimal? rateBn, 
            decimal? rateDdr, 
            DateTime? date, 
            IdMap clientId, 
            bool changedByBot, 
            byte? balanceCurrencyId, 
            bool? isSupplier, 
            bool? isCustomer)
        {
            Id = id;
            RateNal = rateNal;
            RateBn = rateBn;
            RateDdr = rateDdr;
            Date = date;
            ClientId = clientId;
            ChangedByBot = changedByBot;
            BalanceCurrencyId = balanceCurrencyId;
            IsSupplier = isSupplier;
            IsCustomer = isCustomer;
        }

        public MappingInfo IsMappingsFull()
        {
            var isMappingsFull = true;
            var why = new StringBuilder();

            if (ClientId != null && !ClientId.ExternalId.HasValue)
            {
                why.AppendLine($"Поле: ClientId. Id: {ClientId?.InnerId}");
                isMappingsFull = false;
            }

            return new MappingInfo
            {
                IsMappingFull = isMappingsFull,
                Why = why.ToString()
            };
        }
    }
}
