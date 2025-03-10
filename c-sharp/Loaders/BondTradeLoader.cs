using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class BondTradeLoader : ITradeLoader
    {
        public string DataFile { get; set; }

        public IEnumerable<ITrade> LoadTrades()
        {
            var trades = new List<ITrade>();

            var lines = File.ReadAllLines(DataFile);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');

                if (parts.Length < 7)
                    continue;

                string type = parts[0].Trim();
                string tradeType;
                if (type.Equals("GovBond", StringComparison.OrdinalIgnoreCase) ||
                    type.Equals("Supra", StringComparison.OrdinalIgnoreCase))
                {
                    tradeType = BondTrade.GovBondTradeType;
                }
                else if (type.Equals("CorpBond", StringComparison.OrdinalIgnoreCase))
                {
                    tradeType = BondTrade.CorpBondTradeType;
                }
                else
                {
                    continue;
                }

                DateTime tradeDate = DateTime.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
                string instrument = parts[2].Trim();
                string counterparty = parts[3].Trim();
                double notional = double.Parse(parts[4].Trim(), CultureInfo.InvariantCulture);
                double rate = double.Parse(parts[5].Trim(), CultureInfo.InvariantCulture);
                string tradeId = parts[6].Trim();

                var bondTrade = new BondTrade(tradeId, tradeType)
                {
                    TradeDate = tradeDate,
                    Instrument = instrument,
                    Counterparty = counterparty,
                    Notional = notional,
                    Rate = rate
                };

                trades.Add(bondTrade);
            }

            return trades;
        }
    }
}
