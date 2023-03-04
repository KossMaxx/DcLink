using System;

namespace MessageBus.ActivityTypes.Export.Change
{
    public class ChangeActivityTypeMessage : BaseSagaMessage<ActivityTypeDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
