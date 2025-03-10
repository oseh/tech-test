using System;

namespace HmxLabs.TechTest.Models
{
    public class BondTrade : BaseTrade
    {
        private readonly string _tradeType;

        // New constructor takes both trade ID and trade type.
        public BondTrade(string tradeId, string tradeType)
        {
            if (string.IsNullOrWhiteSpace(tradeId))
            {
                throw new ArgumentException("A valid non null, non empty trade ID must be provided");
            }
            
            TradeId = tradeId;
            _tradeType = tradeType;
        }

        public const string GovBondTradeType = "GovBond";
        public const string CorpBondTradeType = "CorpBond";

        // Now returns the type passed into the constructor.
        public override string TradeType { get { return _tradeType; } }
    }
}
