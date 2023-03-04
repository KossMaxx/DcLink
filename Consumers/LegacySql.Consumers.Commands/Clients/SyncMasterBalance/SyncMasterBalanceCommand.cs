using MessageBus.Clients.Import;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Consumers.Commands.Clients
{
    public class SyncMasterBalanceCommand : BaseSaveErpCommand<ErpPartnerDto>
    {
        public SyncMasterBalanceCommand(ErpPartnerDto value, Guid messageId)
            : base(value, messageId)
        {
        }
    }
}
