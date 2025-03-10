using System;

namespace HmxLabs.TechTest.Models
{
    public class FxTrade : BaseTrade
    {
        private readonly string _tradeType;
        
        public FxTrade(string tradeId, string tradeType)
        {
            if (string.IsNullOrWhiteSpace(tradeId))
                throw new ArgumentException("A valid trade ID must be provided", nameof(tradeId));
            TradeId = tradeId;
            _tradeType = tradeType;
        }
        
        public const string FxSpotTradeType = "FxSpot";
        public const string FxForwardTradeType = "FxForward";
        
        public override string TradeType => _tradeType;
        public DateTime ValueDate { get; set; }
    }
}
