using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Building;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Providers;
using JimenaTools.Extensions.Validations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionLocalizationExtensions
    {
        public static IServiceCollection AddInterfacedLocalizations(this IServiceCollection services, Action<ILocalizationOptions> configure = null)
        {
            PrvLocalizationOptions options;
            IReadOnlyCollection<Type> localizationInterfaces;
            IReadOnlyCollection<IError> wrongMembers;

            options = new PrvLocalizationOptions(services.Add);
            configure?.Invoke(options);
            options.AssemblyBuilderType = options.AssemblyBuilderType ?? typeof(DefaultAssemblyBuilder);
            options.ServiceLifetime = options.ServiceLifetime ?? ServiceLifetime.Singleton;
            options.CurrentCultureProvider = options.CurrentCultureProvider ?? typeof(PrvCurrentCultureProvider);
            options.CacheService = options.CacheService ?? typeof(DefaultLocalizationsCacheService);

            if (options.Assemblies.Count == 0)
                options.Assemblies.Add(Assembly.GetCallingAssembly());

            localizationInterfaces = options
                .Assemblies
                .SelectMany(ass => ass.ExportedTypes)
                .FilterCandidateInterfaces()
                .ToList();

            wrongMembers = localizationInterfaces
                .SelectMany(TypeValidationExtensions.ValidateLocalizationInterface)
                .ToList();

            if (wrongMembers.Any())
                throw new InvalidInterfacesLocalizationException(wrongMembers);

            services.Add(new ServiceDescriptor(typeof(IAssemblyBuilder), options.AssemblyBuilderType, ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(typeof(ICurrentCultureProvider), options.CurrentCultureProvider, ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(typeof(ILocalizationsCacheService), options.CacheService, ServiceLifetime.Singleton));
            services.AddLocalizationsCultureConfiguration(options.Cultures);
            services.AddSingleton<ILocalizationsAssembliesConfiguration>(sp => new PrvLocalizationsAssembliesConfiguration(options.Assemblies));
            services.AddLocalizationsProviderConfiguration(options.Providers, options.ReadOnly);

            foreach (Type localizationInterface in localizationInterfaces)
                services.AddInterfaceDescriptor(localizationInterface, options.ServiceLifetime.Value);

            return services;
        }

        public static ILocalizationOptions SetAssemblyOf<T>(this ILocalizationOptions options)
        {
            options.SetAssembly(typeof(T).Assembly);
            return options;
        }

        public static ILocalizationOptions SetCulture(this ILocalizationOptions options, CultureInfo culture)
        {
            options.SetCulture(culture, false);
            return options;
        }

        public static ILocalizationOptions SetDefaultCulture(this ILocalizationOptions options, CultureInfo culture)
        {
            options.SetCulture(culture, true);
            return options;
        }

        private class PrvLocalizationOptions : ILocalizationOptions
        {
            private readonly Dictionary<int, Type> providers;
            private readonly Action<ServiceDescriptor> addService;

            public PrvLocalizationOptions(Action<ServiceDescriptor> addService)
            {
                this.Assemblies = new HashSet<Assembly>();
                this.providers = new Dictionary<int, Type>();
                this.Cultures = new Dictionary<CultureInfo, bool>();
                this.addService = addService;
            }

            public ServiceLifetime? ServiceLifetime { get; set; }

            public HashSet<Assembly> Assemblies { get; }

            public IReadOnlyDictionary<int, Type> Providers => this.providers;

            public Dictionary<CultureInfo, bool> Cultures { get; }

            public Type AssemblyBuilderType { get; set; }

            public Type CurrentCultureProvider { get; set; }

            public Type CacheService { get; set; }

            public bool ReadOnly { get; set; }

            public void SetAssembly(Assembly assembly)
            {
                assembly.ShouldBeNotNull(nameof(assembly));
                this.Assemblies.Add(assembly);
            }

            public void SetProvider<TProvider>(int priority) where TProvider : ILocalizationsProvider
            {
                if (this.providers.ContainsKey(priority))
                    throw new ProviderPriorityArgumentException(priority, typeof(TProvider));

                this.providers[priority] = typeof(TProvider);
            }

            public void SetBuilder<TAssemblyBuilder>() where TAssemblyBuilder : IAssemblyBuilder
            {
                this.AssemblyBuilderType = typeof(TAssemblyBuilder);
            }

            public void SetCache<TCacheService>() where TCacheService : ILocalizationsCacheService
            {
                this.CacheService = typeof(TCacheService);
            }

            public void SetCulture(CultureInfo culture, bool defaultCulture)
            {
                culture.ShouldBeNotNull(nameof(culture));
                this.Cultures[culture] = defaultCulture;
            }

            public void SetCurrentCultureProvider<TCurremtCultureProvider>() where TCurremtCultureProvider : ICurrentCultureProvider
            {
                this.CurrentCultureProvider = typeof(TCurremtCultureProvider);
            }

            public void AddService(Type serviceType, object instance)
            {
                serviceType.ShouldBeNotNull(nameof(serviceType));
                instance.ShouldBeNotNull(nameof(instance));
                this.addService(new ServiceDescriptor(serviceType, instance));
            }

            public void SetSingletonServiceLifetime()
            {
                this.ServiceLifetime = DependencyInjection.ServiceLifetime.Singleton;
            }

            public void SetScopedServiceLifetime()
            {
                this.ServiceLifetime = DependencyInjection.ServiceLifetime.Scoped;
            }

            public void SetTransientServiceLifetime()
            {
                this.ServiceLifetime = DependencyInjection.ServiceLifetime.Transient;
            }
        }

        private class PrvLocalizationsAssembliesConfiguration : ILocalizationsAssembliesConfiguration
        {
            public PrvLocalizationsAssembliesConfiguration(IEnumerable<Assembly> assemblies)
            {
                this.Assemblies = assemblies;
            }

            public IEnumerable<Assembly> Assemblies { get; }
        }

        private class PrvCurrentCultureProvider : ICurrentCultureProvider
        {
            public bool TryGetCurrent(out CultureInfo culture)
            {
                culture = null;
                return false;
            }
        }
    }
}
