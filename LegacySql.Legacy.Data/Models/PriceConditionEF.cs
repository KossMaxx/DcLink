using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegacySql.Legacy.Data.Models
{
    [Table("kolonkaByKlient")]
    public class PriceConditionEF
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int? ClientId { get; set; }
        public int? ProductTypeId { get; set; }
        public string Vendor { get; set; }
        public string ProductManager { get; set; }
        public short? PriceType { get; set; }
        public DateTime? DateTo { get; set; }
        public string Comment { get; set; }
        public decimal? Value { get; set; }
        public decimal? PercentValue { get; set; }
        public int? UpperThresholdPriceType { get; set; }
    }
}
