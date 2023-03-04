using System;
using System.Collections.Generic;
using System.Globalization;

namespace InjectedLocalizations.Abstractions
{
    public abstract class AbstractLocalizations { }

    public abstract class AbstractLocalizations<T> : AbstractLocalizations, ILocalizations where T : ILocalizations
    {
        protected T instance;

        public AbstractLocalizations(IServiceProvider serviceProvider, IDictionary<CultureInfo, Type> localizationTypes)
        {
            this.instance = (T)serviceProvider.CreateInstance(localizationTypes);
        }

        public CultureInfo Culture => this.instance.Culture;

    }
}
