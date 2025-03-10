namespace HmxLabs.TechTest.Models
{
    public interface IPricingEngine
    {
        public abstract void Price(ITrade trade_, IScalarResultReceiver resultReceiver_);
    }
}
