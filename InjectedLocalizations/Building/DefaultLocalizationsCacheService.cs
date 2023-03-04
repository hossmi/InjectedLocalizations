using System;
using System.Collections.Generic;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Building
{
    public class DefaultLocalizationsCacheService : ILocalizationsCacheService
    {
        private readonly IDictionary<Type, Type> types;
        private readonly object semaphore;

        public DefaultLocalizationsCacheService()
        {
            this.semaphore = new object();
            this.types = new Dictionary<Type, Type>();
        }

        public void Set(Type localizationInterface, Type newType)
        {
            lock (this.semaphore)
                this.types[localizationInterface] = newType;
        }

        public bool TryGetValue(Type localizationInterface, out Type newType)
        {
            lock (this.semaphore)
                return this.types.TryGetValue(localizationInterface, out newType);
        }
    }
}
