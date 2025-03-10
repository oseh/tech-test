using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class FxTradeLoader : ITradeLoader
    {
        public string? DataFile { get; set; }

        public IEnumerable<ITrade> LoadTrades()
        {
            if (string.IsNullOrEmpty(DataFile))
                throw new InvalidOperationException("DataFile must be set.");

            string filePath = DataFile;
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            }
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file '{filePath}' was not found.");

            int lineNumber = 0;
            foreach (var line in File.ReadLines(filePath))
            {
                lineNumber++;
                // Assume line 1 is a metadata header and line 2 is column headers.
                if (lineNumber <= 2)
                    continue;

                if (line.StartsWith("END", StringComparison.OrdinalIgnoreCase))
                    break;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                // Use the '¬' delimiter.
                var parts = line.Split('¬');
                if (parts.Length < 9)
                    continue; // skip invalid records

                // Determine trade type
                string type = parts[0].Trim();
                string tradeType;
                if (type.Equals("FxSpot", StringComparison.OrdinalIgnoreCase))
                {
                    tradeType = FxTrade.FxSpotTradeType;
                }
                else if (type.Equals("FxFwd", StringComparison.OrdinalIgnoreCase) ||
                         type.Equals("FxForward", StringComparison.OrdinalIgnoreCase))
                {
                    tradeType = FxTrade.FxForwardTradeType;
                }
                else
                {
                    continue; // unknown type, skip
                }

                // Parse fields (assuming columns: Type, TradeDate, Ccy1, Ccy2, Amount, Rate, ValueDate, Counterparty, TradeId)
                DateTime tradeDate = DateTime.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
                string ccy1 = parts[2].Trim();
                string ccy2 = parts[3].Trim();
                string instrument = ccy1 + ccy2; // instrument is the concatenation of Ccy1 and Ccy2.
                double notional = double.Parse(parts[4].Trim(), CultureInfo.InvariantCulture);
                double rate = double.Parse(parts[5].Trim(), CultureInfo.InvariantCulture);
                DateTime valueDate = DateTime.Parse(parts[6].Trim(), CultureInfo.InvariantCulture);
                string counterparty = parts[7].Trim();
                string tradeId = parts[8].Trim();

                // Yield the trade immediately.
                yield return new FxTrade(tradeId, tradeType)
                {
                    TradeDate = tradeDate,
                    Instrument = instrument,
                    Counterparty = counterparty,
                    Notional = notional,
                    Rate = rate,
                    ValueDate = valueDate
                };
            }
        }
    }
}
