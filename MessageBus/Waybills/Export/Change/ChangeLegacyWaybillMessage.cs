using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBus.Waybills.Export.Change
{
    public class ChangeLegacyWaybillMessage : BaseSagaMessage<WaybillDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
