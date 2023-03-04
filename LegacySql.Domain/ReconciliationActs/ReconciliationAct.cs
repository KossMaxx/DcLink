using System;
using System.Text;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.ReconciliationActs
{
    public class ReconciliationAct : Mapped
    {
        public IdMap Id { get; }
        public decimal? Sum { get; }
        public DateTime? ChangedAt { get; }
        public IdMap ClientId { get; }
        public bool? IsApproved { get; }

        public ReconciliationAct(bool hasMap, IdMap id, decimal? sum, DateTime? changedAt, IdMap clientId, bool? isApproved) : base(hasMap)
        {
            Id = id;
            Sum = sum;
            ChangedAt = changedAt;
            ClientId = clientId;
            IsApproved = isApproved;
        }

        public MappingInfo IsMappingsFull()
        {
            var why = new StringBuilder();

            void CheckExternalId(IdMap idMap, string name)
            {
                if (idMap != null && idMap.ExternalId == null)
                {
                    why.Append($"Поле {name}.Id: {idMap?.InnerId}\n");
                }
            }

            CheckExternalId(ClientId, nameof(ClientId));

            var whyResult = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyResult),
                Why = whyResult,
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