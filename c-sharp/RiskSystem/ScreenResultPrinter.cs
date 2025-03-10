using System;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class ScreenResultPrinter
    {
        public void PrintResults(ScalarResults results_)
        {
            foreach (var result in results_)
            {
                string tradeId = result.TradeId;
                string resultText = result.Result.HasValue ? result.Result.Value.ToString() : "";
                string errorText = string.IsNullOrEmpty(result.Error) ? "" : result.Error;

                // Print based on which values are present.
                if (!string.IsNullOrEmpty(resultText) && !string.IsNullOrEmpty(errorText))
                {
                    Console.WriteLine($"{tradeId} : {resultText} : {errorText}");
                }
                else if (!string.IsNullOrEmpty(resultText))
                {
                    Console.WriteLine($"{tradeId} : {resultText}");
                }
                else if (!string.IsNullOrEmpty(errorText))
                {
                    Console.WriteLine($"{tradeId} : {errorText}");
                }
                else
                {
                    Console.WriteLine($"{tradeId} :");
                }
            }
        }
    }
}
