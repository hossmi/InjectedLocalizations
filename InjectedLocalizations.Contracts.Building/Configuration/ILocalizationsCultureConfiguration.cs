using System.Collections.Generic;
using System.Globalization;

namespace InjectedLocalizations.Configuration
{
    public interface ILocalizationsCultureConfiguration
    {
        IReadOnlyCollection<CultureInfo> Cultures { get; }
        CultureInfo Default { get; }
    }
}