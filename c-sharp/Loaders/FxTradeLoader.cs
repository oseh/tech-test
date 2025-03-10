using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class FxTradeLoader : ITradeLoader
    {
        private string? _dataFile;
        public string? DataFile
        {
            get => _dataFile;
            set => _dataFile = value;
        }

        public IEnumerable<ITrade> LoadTrades()
        {
            if (string.IsNullOrEmpty(DataFile))
                throw new InvalidOperationException("DataFile property must be set.");

            string filePath = DataFile;
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            }

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file '{filePath}' does not exist.");

            var lines = File.ReadAllLines(filePath);


            for (int i = 2; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("END", StringComparison.OrdinalIgnoreCase))
                    break; 

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split('¬');
                if (parts.Length < 9)
                    continue; 
                string typeField = parts[0].Trim();
                string tradeType;
                if (typeField.Equals("FxSpot", StringComparison.OrdinalIgnoreCase))
                {
                    tradeType = FxTrade.FxSpotTradeType;
                }
                else if (typeField.Equals("FxFwd", StringComparison.OrdinalIgnoreCase) ||
                         typeField.Equals("FxForward", StringComparison.OrdinalIgnoreCase))
                {
                    tradeType = FxTrade.FxForwardTradeType;
                }
                else
                {
                    continue;
                }

                DateTime tradeDate = DateTime.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
                string ccy1 = parts[2].Trim();
                string ccy2 = parts[3].Trim();
                string instrument = ccy1 + ccy2;
                double notional = double.Parse(parts[4].Trim(), CultureInfo.InvariantCulture);
                double rate = double.Parse(parts[5].Trim(), CultureInfo.InvariantCulture);
                DateTime valueDate = DateTime.Parse(parts[6].Trim(), CultureInfo.InvariantCulture);
                string counterparty = parts[7].Trim();
                string tradeId = parts[8].Trim();

                var fxTrade = new FxTrade(tradeId, tradeType)
                {
                    TradeDate = tradeDate,
                    Instrument = instrument,
                    Notional = notional,
                    Rate = rate,
                    ValueDate = valueDate,
                    Counterparty = counterparty
                };

                yield return fxTrade;
            }

        }
    }
}
