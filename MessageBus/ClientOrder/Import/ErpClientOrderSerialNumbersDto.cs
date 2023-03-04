using System;
using System.Collections.Generic;

namespace MessageBus.ClientOrder.Import
{
    public class ErpClientOrderSerialNumbersDto
    {
        public Guid ClientOrderId { get; set; }
        public IEnumerable<ErpClientOrderProductSerialNumbersDto> Products { get; set; }
    }
}
