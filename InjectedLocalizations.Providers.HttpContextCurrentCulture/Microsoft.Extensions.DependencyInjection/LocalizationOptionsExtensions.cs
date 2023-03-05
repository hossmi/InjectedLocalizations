using InjectedLocalizations.Configuration;
using InjectedLocalizations.Providers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalizationOptionsExtensions
    {
        public static ILocalizationOptions SetHttpContextCurrentCultureProvider(this ILocalizationOptions options)
        {
            options.SetCurrentCultureProvider<HttpContextCurrentCultureProvider>();
            return options;
        }
    }
}
