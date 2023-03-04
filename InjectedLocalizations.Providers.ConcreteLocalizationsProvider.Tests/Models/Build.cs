using System.Globalization;
using InjectedLocalizations.Building;

namespace InjectedLocalizations.Models
{
    public static class Build
    {
        public static ILocalizationRequest Request<T>(string culture) where T : ILocalizations
        {
            return new FakeLocalizationRequest
            {
                Culture = new CultureInfo(culture),
                InterfaceType = typeof(T),
            };
        }
    }
}
