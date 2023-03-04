using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Providers;
using JimenaTools.Extensions.Enumerables;
using JimenaTools.Extensions.Types;
using JimenaTools.Extensions.Validations;
using Microsoft.Extensions.DependencyInjection;

namespace InjectedLocalizations.Building
{
    public static class ServiceProviderExtensions
    {
        private static readonly Type[] newTypeConstructorParameters = new Type[] { typeof(IServiceProvider) };

        public static void AddInterfaceDescriptor(this IServiceCollection services
            , Type localizationInterface
            , ServiceLifetime serviceLifetime)
        {
            Func<IServiceProvider, object> factory;
            ServiceDescriptor descriptor;

            factory = serviceProvider =>
            {
                object instance;
                ConstructorInfo constructor;
                Type newType;
                ILocalizationsCacheService cacheService;

                cacheService = serviceProvider.GetService<ILocalizationsCacheService>();
                newType = cacheService.GetOrCreate(localizationInterface, serviceProvider.GetService<IAssemblyBuilder>);

                constructor = newType
                    .GetConstructor(newTypeConstructorParameters)
                    ?? throw new ConstructorNotFoundLocalizationException(newType, newTypeConstructorParameters);

                instance = constructor.Invoke(new object[] { serviceProvider });

                return instance;
            };

            descriptor = new ServiceDescriptor(localizationInterface, factory, serviceLifetime);
            services.Add(descriptor);
        }

        public static IEnumerable<Type> AddDefaultProviderIfMissing(this IEnumerable<Type> providers)
        {
            bool isMissing;
            Type defaultLocalizationsProviderType;

            isMissing = true;
            defaultLocalizationsProviderType = typeof(DefaultLocalizationsProvider);
            providers = providers ?? Enumerable.Empty<Type>();

            foreach (Type provider in providers)
            {
                if (provider == defaultLocalizationsProviderType)
                    isMissing = false;

                yield return provider;
            }

            if (isMissing)
                yield return defaultLocalizationsProviderType;
        }

        public static IServiceCollection AddLocalizationsProviderConfiguration(this IServiceCollection services
            , IReadOnlyDictionary<int, Type> prioritizedProviders
            , bool readOnly)
        {
            IReadOnlyCollection<Type> types = prioritizedProviders
                .OrderByDescending(p => p.Key)
                .Select(p => p.Value)
                .AddDefaultProviderIfMissing()
                .Apply(t => services.AddSingleton(t))
                .ToList();

            services.AddSingleton<ILocalizationsProviderConfiguration>(sp =>
            {
                IReadOnlyCollection<ILocalizationsProvider> providers;

                providers = types
                    .Select(sp.GetRequiredService)
                    .Cast<ILocalizationsProvider>()
                    .ToList();

                if (!readOnly)
                {
                    foreach (ILocalizationsProvider issuer in providers.Where(p => p.CanIssueToWriters))
                        foreach (ILocalizationsProvider writer in providers.Where(p => p.CanWriteFromIssuers))
                            issuer.RegisterWriter(writer);
                }

                return new PrvLocalizationsProviderConfiguration
                {
                    Providers = providers,
                };
            });

            return services;
        }

        public static IServiceCollection AddLocalizationsCultureConfiguration(this IServiceCollection services
            , IReadOnlyCollection<KeyValuePair<CultureInfo, bool>> cultures)
        {
            CultureInfo defaultCulture;
            IReadOnlyCollection<CultureInfo> culturesCollection;

            if (!cultures.ShouldBeNotNull(nameof(cultures)).Any())
                throw new MissingCulturesArgumentException(nameof(cultures));

            if (cultures.Count == 1)
                defaultCulture = cultures
                    .First()
                    .Key;
            else
                defaultCulture = cultures
                    .Where(pair => pair.Value)
                    .Select(pair => pair.Key)
                    .ShouldBeSingle(() => new MissingDefaultCultureArgumentException(cultures));

            culturesCollection = cultures
                .Select(c => c.Key)
                .ToList();

            return services
                .AddSingleton<ILocalizationsCultureConfiguration>(sp => new PrvLocalizationsCultureConfiguration
                {
                    Cultures = culturesCollection,
                    Default = defaultCulture,
                });
        }

        public static Type GetOrCreate(this ILocalizationsCacheService cacheService
            , Type localizationInterface
            , Func<IAssemblyBuilder> getAssemblyBuilder)
        {
            if (!cacheService.TryGetValue(localizationInterface, out Type newType))
            {
                Assembly assemblyResult;

                assemblyResult = getAssemblyBuilder()
                    .ShouldBeNotNull(nameof(getAssemblyBuilder))
                    .Build(localizationInterface);

                newType = assemblyResult
                    .ExportedTypes
                    .Where(t => t.IsClass)
                    .Where(t => !t.IsInterface)
                    .Where(t => !t.IsAbstract)
                    .Where(t => t.Implements(localizationInterface))
                    .ShouldBeSingle(items => new MultipleImplementationsLocalizationException(assemblyResult, localizationInterface, items.ToArray()));

                cacheService.Set(localizationInterface, newType);
            }

            return newType;
        }

        private class PrvLocalizationsCultureConfiguration : ILocalizationsCultureConfiguration
        {
            public IReadOnlyCollection<CultureInfo> Cultures { get; internal set; }
            public CultureInfo Default { get; internal set; }
        }

        private class PrvLocalizationsProviderConfiguration : ILocalizationsProviderConfiguration
        {
            public IEnumerable<ILocalizationsProvider> Providers { get; internal set; }
        }
    }
}
