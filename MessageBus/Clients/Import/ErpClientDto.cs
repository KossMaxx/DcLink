using System;
using System.Collections.Generic;

namespace MessageBus.Clients.Import
{
    public class ErpClientDto
    {
        public Guid Id { get; set; }
        public Guid? MasterId { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public Guid? CityRecipient { get; set; }
        public Guid? MainManagerId { get; set; }
        public Guid? ResponsibleManagerId { get; set; }
        public byte DefaultPriceColumn { get; set; }
        public Guid? DepartmentId { get; set; }
        public IEnumerable<ErpClientWarehousePriorityDto> WarehousePriorities { get; set; }
        public IEnumerable<ErpClientWarehouseAccessDto> WarehouseAccesses { get; set; }
    }
}
