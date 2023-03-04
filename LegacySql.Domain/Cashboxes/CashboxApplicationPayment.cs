using LegacySql.Domain.Shared;
using System;
using System.Text;

namespace LegacySql.Domain.Cashboxes
{
    public class CashboxApplicationPayment : Mapped
    {
        public IdMap Id { get; }
        public DateTime Date { get; }
        public IdMap WriteOffCliectId { get; }
        public IdMap ReceiveClientId { get; }
        public int CurrencyId { get; }
        public decimal Amount { get; }
        public string Description { get; }
        public DateTime? ChangeDate { get; }

        public CashboxApplicationPayment(
            bool hasMap, 
            IdMap id, 
            DateTime date, 
            IdMap writeOffCliectId, 
            IdMap receiveClientId, 
            int currencyId, 
            decimal amount, 
            string description, 
            DateTime? changeDate) : base(hasMap)
        {
            Id = id;
            Date = date;
            WriteOffCliectId = writeOffCliectId;
            ReceiveClientId = receiveClientId;
            CurrencyId = currencyId;
            Amount = amount;
            Description = description;
            ChangeDate = changeDate;
        }

        public MappingInfo IsMappingsFull()
        {
            var why = new StringBuilder();
            var isWriteOffCliectMapFull = WriteOffCliectId?.ExternalId != null;
            if (!isWriteOffCliectMapFull)
            {
                why.Append($"Поле: WriteOffCliectId., Id: {WriteOffCliectId?.InnerId}\n");
            }

            var isReceiveClientMapFull = ReceiveClientId?.ExternalId != null;
            if (!isReceiveClientMapFull)
            {
                why.Append($"Поле: ReceiveClientId., Id: {ReceiveClientId?.InnerId}\n");
            }

            return new MappingInfo
            {
                IsMappingFull = isWriteOffCliectMapFull && isReceiveClientMapFull,
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
