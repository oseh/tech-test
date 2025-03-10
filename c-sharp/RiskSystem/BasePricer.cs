using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public abstract class BasePricer
    {
        protected void LoadPricers()
        {
            var pricingConfigLoader = new PricingConfigLoader { ConfigFile = @"../RiskSystem/PricingConfig/PricingEngines.xml" };
            var pricerConfig = pricingConfigLoader.LoadConfig();

            var engineInstances = new Dictionary<Type, IPricingEngine>();

            foreach (var configItem in pricerConfig)
            {
                string fullyQualifiedTypeName = $"{configItem.TypeName}, {configItem.Assembly}";
                Type? engineType = Type.GetType(fullyQualifiedTypeName);

                if (engineType == null)
                {
                    string[] possiblePaths = {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configItem.Assembly + ".dll"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pricers.dll"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Pricers/bin/Debug/net8.0/Pricers.dll"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Pricers/bin/Debug/net8.0/Pricers.dll")
                    };

                    Assembly? asm = null;
                    foreach (var path in possiblePaths)
                    {
                        if (File.Exists(path))
                        {
                            Console.WriteLine($"Loading assembly from: {path}");
                            asm = Assembly.LoadFrom(path);
                            break;
                        }
                    }

                    if (asm == null)
                    {
                        throw new FileNotFoundException($"Could not find assembly for {configItem.Assembly}. Searched in standard locations.");
                    }

                    engineType = asm.GetType(configItem.TypeName!);
                    if (engineType == null)
                        throw new InvalidOperationException($"Unable to load type: {fullyQualifiedTypeName}");
                }

                if (!engineInstances.TryGetValue(engineType, out IPricingEngine? engineInstance))
                {
                    object? instance = Activator.CreateInstance(engineType);
                    if (instance == null)
                        throw new InvalidOperationException($"Unable to instantiate type: {fullyQualifiedTypeName}");
                    if (!(instance is IPricingEngine pricingEngine))
                        throw new InvalidOperationException($"The type {fullyQualifiedTypeName} does not implement IPricingEngine.");
                    engineInstance = pricingEngine;
                    engineInstances.Add(engineType, engineInstance);
                }

                _pricers[configItem.TradeType!] = engineInstance;
            }
        }

        protected readonly Dictionary<string, IPricingEngine> _pricers = new Dictionary<string, IPricingEngine>();
    }
}