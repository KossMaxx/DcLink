using LegacySql.Domain.Shared;
using System;
using System.Text;

namespace LegacySql.Domain.Firms
{
    public class Firm : Mapped
    {
        public IdMap Id { get; set; }
        public string TaxCode { get; set; }
        public string Title { get; set; }
        public string LegalAddress { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Account { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public bool IsNotResident { get; set; }
        public string PayerCode { get; set; }
        public string CertificateNumber { get; set; }
        public bool? NotVat { get; set; }
        public DateTime? ChangedAt { get; set; }
        public IdMap ClientId { get; set; }
        public IdMap MasterClientId { get; set; }

        public Firm() : base(false) { }

        public Firm(
            IdMap id,
            string taxCode,
            string title,
            string legalAddress,
            string address,
            string phone,
            string account,
            string bankCode,
            string bankName,
            string payerCode,
            string certificateNumber,
            bool? notVat,
            DateTime? changedAt,
            IdMap clientId,
            bool hasMap,
            IdMap masterClientId, 
            bool isNotResident) : base(hasMap)
        {
            Id = id;
            TaxCode = taxCode;
            Title = title;
            LegalAddress = legalAddress;
            Address = address;
            Phone = phone;
            Account = account;
            BankCode = bankCode;
            BankName = bankName;
            PayerCode = payerCode;
            CertificateNumber = certificateNumber;
            NotVat = notVat;
            ChangedAt = changedAt;
            ClientId = clientId;
            MasterClientId = masterClientId;
            IsNotResident = isNotResident;
        }

        public MappingInfo IsMappingsFull()
        {
            var isClientMappingFull = ClientId == null || ClientId.ExternalId != null;
            var isMasterClientMappingFull = MasterClientId == null || MasterClientId.ExternalId != null;

            var isMappingFull = isClientMappingFull && isMasterClientMappingFull;

            if (isMappingFull)
            {
                return new MappingInfo { IsMappingFull = true, Why = "" };
            }

            var why = new StringBuilder();

            if (!isClientMappingFull)
            {
                why.AppendLine($"Поле: ClientId, Id: {ClientId?.InnerId}");
            }

            if (!isMasterClientMappingFull)
            {
                why.AppendLine($"Поле: MasterClientId, Id: {MasterClientId?.InnerId}");
            }

            return new MappingInfo
            {
                IsMappingFull = false,
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
