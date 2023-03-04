using LegacySql.Domain.Shared;
using System;

namespace LegacySql.Domain.BankPayments
{
    public class BankPaymentMap : ExternalMap
    {
        public int? ClientOrderId { get; set; }

        public BankPaymentMap(Guid mapId, int legacyId, int? clientOrderId, Guid? externalMapId = null, Guid? id = null) 
            : base(mapId, legacyId, externalMapId, id)
        {
            ClientOrderId = clientOrderId;
        }
    }
}
