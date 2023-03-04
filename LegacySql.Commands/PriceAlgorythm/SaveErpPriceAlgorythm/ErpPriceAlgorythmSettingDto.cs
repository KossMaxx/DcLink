using System;

namespace LegacySql.Commands.PriceAlgorythm.SaveErpPriceAlgorythm
{
    public class ErpPriceAlgorythmSettingDto
    {
        public int Id { get; set; }
        public Guid? ClientGuid { get; set; }
        public bool Rrp { get; set; }
        public bool Rdp { get; set; }
        public decimal XRate { get; set; }
        public bool Price { get; set; }
        public bool CompetitorOpt { get; set; }
        public bool CompetitorRozn { get; set; }
        public bool Competitor1 { get; set; }
        public bool Competitor1Rozn { get; set; }
    }
}