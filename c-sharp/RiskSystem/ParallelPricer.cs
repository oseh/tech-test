using System.Collections.Generic;
using System.Threading.Tasks;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class ParallelPricer : BasePricer
    {
        public void Price(IEnumerable<IEnumerable<ITrade>> tradeContainers, IScalarResultReceiver resultReceiver)
        {
            LoadPricers();

            var tasks = new List<Task>();
            object resultLock = new object();

            foreach (var container in tradeContainers)
            {
                foreach (var trade in container)
                {
                    if (!_pricers.TryGetValue(trade.TradeType, out IPricingEngine pricer))
                    {
                        lock (resultLock)
                        {
                            resultReceiver.AddError(trade.TradeId, "No Pricing Engines available for this trade type");
                        }
                    }
                    else
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            pricer.Price(trade, resultReceiver);
                        }));
                    }
                }
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}