using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("РН")]
    public class ClientOrderEF
    {
        public int Id { get; set; } //НомерПН
        public DateTime? Date { get; set; } //DataSozd
        public int ClientId { get; set; } //klientID
        public virtual ClientEF Client { get; set; }
        public string Comments { get; set; } //Описание
        public DateTime? ChangedAt { get; set; } //modified_at
        public bool IsExecuted { get; set; } //ф
        public bool IsCashless { get; set; } //from dbo.GetOrderTag(rn.НомерПН)
        public string MarketplaceNumber { get; set; } //customer_order_ID
        public string Manager { get; set; } //менеджер
        public bool IsPaid { get; set; } //Paid
        public int? WarehouseId { get; set; } //Отдел
        public int? Quantity { get; set; } //Кол
        public double? Amount { get; set; } //Сумма
        public DateTime? PaymentDate { get; set; } //DataBal
    }
}
