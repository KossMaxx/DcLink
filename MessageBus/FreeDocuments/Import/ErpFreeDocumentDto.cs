using System;

namespace MessageBus.FreeDocuments.Import
{
    public class ErpFreeDocumentDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public Guid ClientId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
