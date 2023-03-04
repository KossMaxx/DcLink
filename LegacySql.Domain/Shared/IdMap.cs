using System;

namespace LegacySql.Domain.Shared
{
    public class IdMap
    { 
        public int InnerId { get; }
        public Guid? ExternalId { get; }

        public IdMap(int innerId)
        {
            InnerId = innerId;
        }

        public IdMap(int innerId, Guid? externalId) : this(innerId)
        {
            ExternalId = externalId;
        }
    }
}
