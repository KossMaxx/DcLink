using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Queries.SerialNumbers
{
    public class SerialNumberDto
    {
        public IEnumerable<SerialNumberPurchaseDto> Purchases { get; set; } = new List<SerialNumberPurchaseDto>();
        public IEnumerable<SerialNumberSaleDto> Sales { get; set; } = new List<SerialNumberSaleDto>();

        public class SerialNumberSaleDto
        {
            public DateTime Date { get; set; }
            public string Number { get; set; }
            public Guid? ClientId { get; set; }
            public int ClientSqlId { get; set; }
            public Guid? ProductId { get; set; }
            public int ProductSqlId { get; set; }
            public decimal? ProductPrice { get; set; }
        }

        public class SerialNumberPurchaseDto
        {
            public DateTime Date { get; set; }
            public string Number { get; set; }
            public Guid? ClientId { get; set; }
            public int ClientSqlId { get; set; }
            public Guid? ProductId { get; set; }
            public int ProductSqlId { get; set; }
            public decimal? ProductPrice { get; set; }
        }
    }
}
