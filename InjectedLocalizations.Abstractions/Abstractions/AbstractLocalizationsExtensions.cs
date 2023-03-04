using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Abstractions
{
    public static class AbstractLocalizationsExtensions
    {
        public static object CreateInstance(this IServiceProvider serviceProvider, IDictionary<CultureInfo, Type> localizationTypes)
        {
            object instance;
            ILocalizationsCultureConfiguration cultureConfiguration;
            ICurrentCultureProvider currentCultureProvider;
            ConstructorInfo defaultConstructor;

            serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            localizationTypes = localizationTypes ?? throw new ArgumentNullException(nameof(localizationTypes));
            cultureConfiguration = (ILocalizationsCultureConfiguration)serviceProvider.GetService(typeof(ILocalizationsCultureConfiguration));
            currentCultureProvider = (ICurrentCultureProvider)serviceProvider.GetService(typeof(ICurrentCultureProvider));

            if (!currentCultureProvider.TryGetCurrent(out CultureInfo currentCulture))
                currentCulture = cultureConfiguration.Default;

            if (!localizationTypes.TryGetValue(currentCulture, out Type concreteType))
                throw new NotFoundConcreteTypeForSelectedCultureLocalizationException(currentCulture);

            defaultConstructor = concreteType
                .GetConstructor(new Type[0])
                ?? throw new ConstructorNotFoundLocalizationException(concreteType, new Type[0]);

            instance = defaultConstructor.Invoke(new object[0]);

            return instance;
        }
    }
}
