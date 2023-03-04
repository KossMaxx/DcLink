using System;
using System.Collections.Generic;

namespace LegacySql.Queries.PriceAlgorythms.PreliminaryPrices
{
    public class PreliminaryPriceAlgorythmDto
    {
        public Guid? ProductGuid { get; set; }
        public long ProductId { get; set; }
        
        public long PriceAlgorythmId { get; set; }
        
        public string Mark { get; set; }
        
        public decimal? Ss { get; set; }
        
        public decimal? PriceUahMin { get; set; }
        
        public decimal? PriceUahRdp { get; set; }
        
        public decimal? PriceRrp { get; set; }
        
        public decimal? KonkurentOpt { get; set; }
        
        public decimal? KonkurentRozn { get; set; }

        #region Opt4
        public decimal? Opt4X { get; set; }

        public decimal? Opt4A { get; set; }

        public int Opt4Type { get; set; }

        public int Opt4Competitor { get; set; }

        public decimal? Opt4End { get; set; }

        public decimal? Price4 { get; set; }

        public decimal? Opt4EndX { get; set; }
        #endregion

        #region Opt5
        public decimal? Opt5X { get; set; }

        public decimal? Opt5A { get; set; }

        public int Opt5Type { get; set; }

        public int Opt5Competitor { get; set; }

        public decimal? Opt5End { get; set; }

        public decimal? Price5 { get; set; }

        public decimal? Opt5EndX { get; set; }
        #endregion

        #region Opt0
        public decimal? Opt0X { get; set; }

        public decimal? Opt0A { get; set; }

        public int Opt0Type { get; set; }

        public int Opt0Competitor { get; set; }

        public decimal? Opt0End { get; set; }

        public decimal? Price0 { get; set; }

        public decimal? Opt0EndX { get; set; }
        #endregion

        #region Opt1
        public decimal? Opt1X { get; set; }

        public decimal? Opt1A { get; set; }

        public int Opt1Type { get; set; }

        public int Opt1Competitor { get; set; }

        public decimal? Opt1End { get; set; }

        public decimal? Price1 { get; set; }

        public decimal? Opt1EndX { get; set; }
        #endregion

        #region OptI
        public decimal? OptIX { get; set; }

        public decimal? OptIA { get; set; }

        public int OptIType { get; set; }

        public int OptICompetitor { get; set; }

        public decimal? OptIEnd { get; set; }

        public decimal? PriceI { get; set; }

        public decimal? OptIEndX { get; set; }
        #endregion
        
        public decimal? RateT { get; set; }
        
        public decimal? MinRent { get; set; }

        public int Nal { get; set; }
        
        public bool ActiveRrp { get; set; }
        
        public decimal? Price3 { get; set; }
        
        public decimal? PriceUah { get; set; }

        #region OptPriceUah
        public decimal? PriceUahX { get; set; }

        public decimal? PriceUahA { get; set; }

        public int PriceUahType { get; set; }

        public int PriceUahCompetitor { get; set; }

        public decimal? PriceUahEnd { get; set; }

        public decimal? PriceUahEndX { get; set; }
        #endregion
        
        public decimal? RrpX { get; set; }

        public decimal? RrpA { get; set; }
        
        public decimal? KonkurentOpt1 { get; set; }
        
        public bool ManualRrp { get; set; }

        public decimal? KonkurentOpt1Rozn { get; set; }
    }
}
