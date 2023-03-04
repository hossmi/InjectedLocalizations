using System.Collections.Generic;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Configuration
{
    public interface ILocalizationsProviderConfiguration
    {
        IEnumerable<ILocalizationsProvider> Providers { get; }
    }
}