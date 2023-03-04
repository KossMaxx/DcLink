using System;
using System.Collections.Generic;

namespace MessageBus.ClientOrder.Import
{
    public class ErpClientOrderProductSerialNumbersDto
    {
        public Guid ProductId { get; set; }
        public IEnumerable<string> SerialNumbers { get; set; }
    }
}
