using InjectedLocalizations.Configuration;
using InjectedLocalizations.Providers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalizationOptionsExtensions
    {
        public static ILocalizationOptions UseConcreteProvider(this ILocalizationOptions options, int priority = int.MaxValue)
        {
            options.SetProvider<ConcreteLocalizationsProvider>(priority);

            return options;
        }
    }
}
