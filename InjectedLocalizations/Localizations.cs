using System;
using JimenaTools.Extensions.Validations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InjectedLocalizations
{
    public static class Localizations
    {
        private static IServiceProvider serviceProvider;

        internal static IServiceProvider ServiceProvider
        {
            set => serviceProvider = value.ShouldBeNotNull(nameof(ServiceProvider));
        }

        public static T For<T>() where T : ILocalizations
        {
            T instance;

            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(ServiceProvider), $"{nameof(ServiceProvider)} is null. Please, add {nameof(ApplicationBuilderLocalizationExtensions.AddStaticLocalizations)}() to {nameof(IApplicationBuilder)} at Startup Configure method.");

            instance = serviceProvider.GetService<T>();

            return instance;
        }
    }
}
