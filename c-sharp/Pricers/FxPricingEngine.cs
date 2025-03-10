namespace HmxLabs.TechTest.Pricers
{
    public class FxPricingEngine : BasePricingEngine
    {
        public FxPricingEngine()
        {
            Delay = 2000;
            AddSupportedTradeType("FxSpot");
            AddSupportedTradeType("FxForward");
        }
    }
}