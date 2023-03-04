using System;

namespace MessageBus.MovementOrders.Import
{
    public class ErpMovementOrderDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public decimal Amount { get; set; }
    }
}
