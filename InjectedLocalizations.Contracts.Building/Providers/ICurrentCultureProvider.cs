using System.Globalization;

namespace InjectedLocalizations.Providers
{
    public interface ICurrentCultureProvider
    {
        bool TryGetCurrent(out CultureInfo culture);
    }
}