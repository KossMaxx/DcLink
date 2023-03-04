using System;
using MessageBus.Employees;

namespace MessageBus.PhysicalPersons.Export.Change
{
    public class ChangeLegacyPhysicalPersonMessage : BaseSagaMessage<PhysicalPersonDto>, IMappedMessage
    {
        public Guid ErpId { get; set; }
    }
}
