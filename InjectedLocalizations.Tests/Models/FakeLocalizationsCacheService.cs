using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Models
{
    public class FakeLocalizationsCacheService : ILocalizationsCacheService
    {
        public FakeLocalizationsCacheService()
        {
            this.Types = new Dictionary<Type, Type>();
        }

        public IDictionary<Type, Type> Types { get; set; }

        public void Set(Type localizationInterface, Type newType)
        {
            this.Types[localizationInterface] = newType;
        }

        public bool TryGetValue(Type localizationInterface, out Type newType)
        {
            return this.Types.TryGetValue(localizationInterface, out newType);
        }
    }
}
