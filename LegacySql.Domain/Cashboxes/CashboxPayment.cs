using LegacySql.Domain.Shared;
using System;
using System.Text;

namespace LegacySql.Domain.Cashboxes
{
    public class CashboxPayment : Mapped
    {
        public IdMap Id { get; }
        public DateTime Date { get; }
        public IdMap CashboxId { get; }
        public IdMap ClientId { get; }
        public decimal AmountUSD { get; }
        public decimal AmountUAH { get; }
        public decimal AmountEuro { get; }
        public decimal Rate { get; }
        public decimal RateEuro { get; }
        public string Description { get; }
        public decimal Total { get; }
        public DateTime? ChangedAt { get; }

        public CashboxPayment(
            bool hasMap,
            IdMap id,
            DateTime date,
            IdMap cashboxId,
            IdMap clientId,
            decimal amountUSD,
            decimal amountUAH,
            decimal amountEuro,
            decimal rate,
            decimal rateEuro,
            string description,
            decimal total, 
            DateTime? changedAt) : base(hasMap)
        {
            Id = id;
            Date = date;
            CashboxId = cashboxId;
            ClientId = clientId;
            AmountUSD = amountUSD;
            AmountUAH = amountUAH;
            AmountEuro = amountEuro;
            Rate = rate;
            RateEuro = rateEuro;
            Description = description;
            Total = total;
            ChangedAt = changedAt;
        }

        public MappingInfo IsMappingsFull()
        {
            var why = new StringBuilder();
            var isClientMapFull = ClientId?.ExternalId != null;
            if (!isClientMapFull)
            {
                why.Append($"Поле: ClientId., Id: {ClientId?.InnerId}\n");
            }

            var isCashboxMapFull = CashboxId?.ExternalId != null;
            if (!isCashboxMapFull)
            {
                why.Append($"Поле: CashboxId., Id: {CashboxId?.InnerId}\n");
            }

            return new MappingInfo
            {
                IsMappingFull = isClientMapFull && isCashboxMapFull,
                Why = why.ToString()
            };
        }

        public bool IsNew()
        {
            return !HasMap;
        }

        public bool IsChanged()
        {
            return Id?.ExternalId != null;
        }
    }
}
