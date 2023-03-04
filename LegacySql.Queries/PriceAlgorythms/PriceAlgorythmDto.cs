using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Queries.PriceAlgorythms
{
    public class PriceAlgorythmDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ManagerLogin { get; set; }
        public int? ManagerId { get; set; }
        public Guid? ManagerGuid { get; set; }

        public bool Active { get; set; }

        public bool ZakazOnly { get; set; }

        public bool ActiveRrp { get; set; }

        public int? ClientId { get; set; }
        public Guid? ClientGuid { get; set; }

        public bool? ActivePriceUah { get; set; }

        public bool ZakazOnlyRrp { get; set; }

        public decimal MinRent { get; set; }

        public decimal RrpX { get; set; }

        public decimal RrpA { get; set; }

        public decimal RrpC { get; set; }

        public int AlgorithmNalType { get; set; }

        public bool MinRentUseSklad { get; set; }

        public bool DoNonDeleteRrp { get; set; }

        #region Opt4
        public decimal Opt4X { get; set; }

        public decimal Opt4A { get; set; }

        public int Opt4Type { get; set; }

        public int Opt4Competitor { get; set; }

        public decimal Opt4End { get; set; }

        public decimal Opt4EndX { get; set; }
        #endregion

        #region Opt5
        public decimal Opt5X { get; set; }

        public decimal Opt5A { get; set; }

        public int Opt5Type { get; set; }

        public int Opt5Competitor { get; set; }

        public decimal Opt5End { get; set; }

        public decimal Opt5EndX { get; set; }
        #endregion

        #region Opt0
        public decimal Opt0X { get; set; }

        public decimal Opt0A { get; set; }

        public int Opt0Type { get; set; }

        public int Opt0Competitor { get; set; }

        public decimal Opt0End { get; set; }

        public decimal Opt0EndX { get; set; }
        #endregion

        #region Opt1
        public decimal Opt1X { get; set; }

        public decimal Opt1A { get; set; }

        public int Opt1Type { get; set; }

        public int Opt1Competitor { get; set; }

        public decimal Opt1End { get; set; }

        public decimal Opt1EndX { get; set; }
        #endregion

        #region OptI
        public decimal OptIX { get; set; }

        public decimal OptIA { get; set; }

        public int OptIType { get; set; }

        public int OptICompetitor { get; set; }

        public decimal OptIEnd { get; set; }

        public decimal OptIEndX { get; set; }
        #endregion

        #region OptPriceUah
        public decimal PriceUahX { get; set; }

        public decimal PriceUahA { get; set; }

        public int PriceUahType { get; set; }

        public int PriceUahCompetitor { get; set; }

        public decimal PriceUahEnd { get; set; }

        public decimal PriceUahEndX { get; set; }
        #endregion

        public IEnumerable<PriceAlgorythmDetailDto> Details { get; set; }
        public IEnumerable<PriceAlgorythmSettingDto> Settings { get; set; }
    }
}
