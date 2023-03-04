using System.Collections.Generic;
using System.Reflection;

namespace InjectedLocalizations.Configuration
{
    public interface ILocalizationsAssembliesConfiguration
    {
        IEnumerable<Assembly> Assemblies { get; }
    }
}