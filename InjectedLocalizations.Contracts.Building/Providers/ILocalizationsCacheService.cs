using System;

namespace InjectedLocalizations.Providers
{
    public interface ILocalizationsCacheService
    {
        void Set(Type localizationInterface, Type newType);
        bool TryGetValue(Type localizationInterface, out Type newType);
    }
}