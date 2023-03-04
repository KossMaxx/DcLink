using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LegacySql.Domain.Purchases;

namespace LegacySql.Legacy.Data.Models
{
    [Table("ПН")]
    public class PurchaseEF
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsExecuted { get; set; }
        public string Comments { get; set; }
        public byte Type { get; set; }
        public bool IsActual { get; set; }
        public decimal? TransportationCost { get; set; }
        public byte? CostType { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsFinancialSideConfirmed { get; set; }
        public bool? IsProductsArrivedToPort { get; set; }
        public bool IsCashlessDocumentsProcessNeeded { get; set; }
        public string SupplierDocument { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? ChangedAt { get; set; }
        public bool IsPaid { get; set; }
        public string EmployeeUsername { get; set; }
        public DateTime? PaymentDate { get; set; }      
        public int? WarehouseId { get; set; }
        public int ClientId { get; set; }
        public virtual ICollection<PurchaseItemEF> Items { get; set; }
    }
}
