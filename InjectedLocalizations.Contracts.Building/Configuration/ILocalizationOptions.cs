using System;
using System.Globalization;
using System.Reflection;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Configuration
{
    public interface ILocalizationOptions
    {
        bool ReadOnly { get; set; }

        void AddService(Type serviceType, object instance);
        void SetAssembly(Assembly assembly);
        void SetProvider<TProvider>(int priority) where TProvider : ILocalizationsProvider;
        void SetCulture(CultureInfo culture, bool defaultCulture);
        void SetBuilder<TAssemblyBuilder>() where TAssemblyBuilder : IAssemblyBuilder;
        void SetCurrentCultureProvider<TCurrentCultureProvider>() where TCurrentCultureProvider : ICurrentCultureProvider;
        void SetCache<TCacheService>() where TCacheService : ILocalizationsCacheService;
        void SetSingletonServiceLifetime();
        void SetScopedServiceLifetime();
        void SetTransientServiceLifetime();
    }
}
