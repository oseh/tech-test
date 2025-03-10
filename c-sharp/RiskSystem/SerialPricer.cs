using System.Collections.Generic;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class SerialPricer : BasePricer
    {
        public void Price(IEnumerable<IEnumerable<ITrade>> tradeContainers, IScalarResultReceiver resultReceiver)
        {
            LoadPricers();

            foreach (var tradeContainer in tradeContainers)
            {
                foreach (var trade in tradeContainer)
                {
                    if (!_pricers.ContainsKey(trade.TradeType))
                    {
                        resultReceiver.AddError(trade.TradeId, "No Pricing Engines available for this trade type");
                        continue;
                    }

                    var pricer = _pricers[trade.TradeType];
                    pricer.Price(trade, resultReceiver);
                }
            }
        }
    }
}