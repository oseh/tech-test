using System;
using System.IO;
using System.Xml.Linq;
using HmxLabs.TechTest.Models; // For FxTrade.FxForwardTradeType
using HmxLabs.TechTest.RiskSystem; // For PricingEngineConfig and PricingEngineConfigItem

namespace HmxLabs.TechTest.RiskSystem
{
    public class PricingConfigLoader
    {
        public string? ConfigFile { get; set; }

        public PricingEngineConfig LoadConfig()
        {
            if (string.IsNullOrEmpty(ConfigFile))
                throw new InvalidOperationException("ConfigFile property must be set.");

            // Resolve the file path relative to the current directory.
            string filePath = ConfigFile;
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            }

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The configuration file '{filePath}' was not found.");

            // Load the XML document.
            XDocument doc = XDocument.Load(filePath);
            var config = new PricingEngineConfig();

            // Iterate over each <Engine> element in the XML.
            foreach (var engine in doc.Root?.Elements("Engine") ?? Array.Empty<XElement>())
            {
                string tradeTypeAttr = engine.Attribute("tradeType")?.Value ?? "";
                string assemblyAttr = engine.Attribute("assembly")?.Value ?? "";
                string pricingEngineAttr = engine.Attribute("pricingEngine")?.Value ?? "";

                // Map "FxFwd" to FxTrade.FxForwardTradeType; otherwise, use the attribute value.
                string tradeType = tradeTypeAttr.Equals("FxFwd", StringComparison.OrdinalIgnoreCase)
                    ? FxTrade.FxForwardTradeType
                    : tradeTypeAttr;

                var item = new PricingEngineConfigItem
                {
                    TradeType = tradeType,
                    Assembly = assemblyAttr,
                    TypeName = pricingEngineAttr
                };

                config.Add(item);
            }

            return config;
        }
    }
}
