using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("РН_Arch")]
    public class ClientOrderArchivalEF
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int ClientId { get; set; }
        public virtual ClientEF Client { get; set; }
        public string Comments { get; set; }
        public DateTime? ChangedAt { get; set; }
        public bool IsExecuted { get; set; }
        public bool IsCashless { get; set; }
        public string MarketplaceNumber { get; set; }
        public string Manager { get; set; }
        public bool IsPaid { get; set; }
        public int? WarehouseId { get; set; }
        public int? Quantity { get; set; }
        public double? Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
