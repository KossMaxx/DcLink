using System;
using System.Collections.Generic;
using System.Text;

namespace MessageBus
{
    public interface IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
