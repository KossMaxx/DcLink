using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Domain.Shared
{
    public class StringIdMap
    {
        public string InnerId { get; }
        public Guid? ExternalId { get; }

        public StringIdMap(string innerId)
        {
            InnerId = innerId;
        }

        public StringIdMap(string innerId, Guid? externalId) : this(innerId)
        {
            ExternalId = externalId;
        }
    }
}
