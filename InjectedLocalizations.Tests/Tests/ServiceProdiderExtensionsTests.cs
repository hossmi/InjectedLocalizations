using System.Globalization;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace InjectedLocalizations.Tests
{
    public class ServiceProdiderExtensionsTests
    {
        [Fact]
        public void Can_add_interface_descriptor()
        {
            FakeServiceCollection services;
            ServiceDescriptor descriptor;

            services = new FakeServiceCollection();
            services.AddInterfaceDescriptor(typeof(IEmptyLocalizations), ServiceLifetime.Scoped);

            services.Count.Should().Be(1);

            descriptor = services.Single();
            descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
            descriptor.ServiceType.Should().Be(typeof(IEmptyLocalizations));
            descriptor.ImplementationFactory.Should().NotBeNull();
        }

        [Fact]
        public void Cannot_get_service_without_right_constructor()
        {
            FakeServiceCollection services;
            IEmptyLocalizations localizations;
            ServiceProvider provider;
            Action getServiceCall;

            services = new FakeServiceCollection();

            services.AddInterfaceDescriptor(typeof(IEmptyLocalizations), ServiceLifetime.Scoped);
            services.Count.Should().Be(1);

            services.AddSingleton<ILocalizationsCacheService, FakeLocalizationsCacheService>();

            provider = services.BuildServiceProvider();
            provider.GetService<ILocalizationsCacheService>().Set(typeof(IEmptyLocalizations), typeof(EmptyLocalizations));

            getServiceCall = () => localizations = provider.GetService<IEmptyLocalizations>();

            getServiceCall.Should().ThrowExactly<ConstructorNotFoundLocalizationException>();
        }

        [Fact]
        public void Can_get_service_with_right_constructor()
        {
            FakeServiceCollection services;
            IValidLocalizations localizations;
            ServiceProvider serviceProvider;
            FakeLocalizationsCacheService cacheService;

            services = new FakeServiceCollection();

            services.AddInterfaceDescriptor(typeof(IValidLocalizations), ServiceLifetime.Scoped);
            services.Count.Should().Be(1);

            cacheService = new FakeLocalizationsCacheService();
            cacheService.Set(typeof(IValidLocalizations), typeof(ConcreteValidLocalizations));
            services.AddSingleton<ILocalizationsCacheService>(cacheService);
            serviceProvider = services.BuildServiceProvider();

            localizations = serviceProvider.GetService<IValidLocalizations>();

            localizations.Should().NotBeNull();
            localizations.GetType().Should().Be(typeof(ConcreteValidLocalizations));
            localizations.As<ConcreteValidLocalizations>().ServiceProvider.Should().NotBeNull();
        }

        [Fact]
        public void Can_add_and_get_type_from_cache_service()
        {
            FakeLocalizationsCacheService cacheService;
            IAssemblyBuilder assemblyBuilder;
            Type type;

            cacheService = new FakeLocalizationsCacheService();
            assemblyBuilder = Substitute.For<IAssemblyBuilder>();

            assemblyBuilder
                .Build(Arg.Is(typeof(IEmptyLocalizations)))
                .Returns(typeof(EmptyLocalizations).Assembly);

            type = cacheService.GetOrCreate(typeof(IEmptyLocalizations), () => assemblyBuilder);

            type.Should().Be(typeof(EmptyLocalizations));
            cacheService.Types.Should().HaveCount(1);
            cacheService.Types.Single().Key.Should().Be(typeof(IEmptyLocalizations));
            cacheService.Types.Single().Value.Should().Be(typeof(EmptyLocalizations));
        }

        [Fact]
        public void Can_get_existing_type_from_cache_service()
        {
            ILocalizationsCacheService cacheService;
            IAssemblyBuilder assemblyBuilder;
            Type type;

            cacheService = new FakeLocalizationsCacheService();
            cacheService.Set(typeof(IEmptyLocalizations), typeof(EmptyLocalizations));

            assemblyBuilder = Substitute.For<IAssemblyBuilder>();
            type = cacheService.GetOrCreate(typeof(IEmptyLocalizations), () => assemblyBuilder);

            type.Should().Be(typeof(EmptyLocalizations));
        }

        [Fact]
        public void AddDefaultProviderIfMissing_adds_provider_to_an_empty_collection()
        {
            Type[] types;

            types = new Type[0]
                .AddDefaultProviderIfMissing()
                .ToArray();

            types.Should().HaveCount(1);
            types[0].Should().Be(typeof(DefaultLocalizationsProvider));
        }

        [Fact]
        public void AddDefaultProviderIfMissing_adds_provider_to_a_collection()
        {
            Type[] types;

            types = new[]
                {
                    typeof(string),
                }
                .AddDefaultProviderIfMissing()
                .OrderBy(t => t.Name)
                .ToArray();

            types.Should().HaveCount(2);
            types[0].Should().Be(typeof(DefaultLocalizationsProvider));
        }

        [Fact]
        public void AddDefaultProviderIfMissing_does_not_add_provider_if_it_exists_at_collection()
        {
            Type[] types;

            types = new[]
                {
                    typeof(string),
                    typeof(DefaultLocalizationsProvider),
                }
                .AddDefaultProviderIfMissing()
                .OrderBy(t => t.Name)
                .ToArray();

            types.Should().HaveCount(2);
            types[0].Should().Be(typeof(DefaultLocalizationsProvider));
        }

        [Fact]
        public void AddDefaultProviderIfMissing_does_not_add_provider_if_it_exists_at_collection_2()
        {
            Type[] types;

            types = new[]
                {
                    typeof(DefaultLocalizationsProvider),
                }
                .AddDefaultProviderIfMissing()
                .OrderBy(t => t.Name)
                .ToArray();

            types.Should().HaveCount(1);
            types[0].Should().Be(typeof(DefaultLocalizationsProvider));
        }

        [Fact]
        public void Can_add_minimal_providers_configuration()
        {
            IServiceCollection services;
            Dictionary<int, Type> providers;
            ServiceDescriptor descriptor;

            services = new FakeServiceCollection();
            providers = new Dictionary<int, Type>();

            services.AddLocalizationsProviderConfiguration(providers, true);

            services.Count.Should().Be(2);

            descriptor = services.Single(d => d.ServiceType == typeof(DefaultLocalizationsProvider));
            descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);

            descriptor = services.Single(d => d.ServiceType == typeof(ILocalizationsProviderConfiguration));
            descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        }

        [Fact]
        public void Can_add_minimal_providers_configuration_and_get_default_provider()
        {
            IServiceCollection services;
            ILocalizationsProviderConfiguration configuration;
            Dictionary<int, Type> providers;
            ServiceProvider serviceProvider;

            services = new FakeServiceCollection();
            providers = new Dictionary<int, Type>();

            services.AddLocalizationsProviderConfiguration(providers, true);

            serviceProvider = services.BuildServiceProvider();
            configuration = serviceProvider.GetService<ILocalizationsProviderConfiguration>();

            configuration.Should().NotBeNull();
            configuration.Providers.Should().HaveCount(1);
            configuration.Providers.Single().GetType().Should().Be(typeof(DefaultLocalizationsProvider));
        }

        [Fact]
        public void Can_add_providers_configuration()
        {
            IServiceCollection services;
            ILocalizationsProviderConfiguration configuration;
            Dictionary<int, Type> providers;
            ServiceProvider serviceProvider;
            ILocalizationsProvider[] instances;

            services = new FakeServiceCollection();
            providers = new Dictionary<int, Type>();
            providers.Add(100, typeof(MinimalFakeLocalizationsProvider));

            services.AddLocalizationsProviderConfiguration(providers, true);

            serviceProvider = services.BuildServiceProvider();
            configuration = serviceProvider.GetService<ILocalizationsProviderConfiguration>();

            configuration.Should().NotBeNull();
            configuration.Providers.Should().HaveCount(2);

            instances = configuration.Providers.ToArray();
            instances[0].GetType().Should().Be(typeof(MinimalFakeLocalizationsProvider));
            instances[1].GetType().Should().Be(typeof(DefaultLocalizationsProvider));
        }

        [Fact]
        public void Cannot_add_culture_confirugation_without_cultures()
        {
            IServiceCollection services;
            Dictionary<CultureInfo, bool> cultures;

            services = new FakeServiceCollection();
            cultures = new Dictionary<CultureInfo, bool>();

            Action action = () => services.AddLocalizationsCultureConfiguration(cultures);

            action.Should().ThrowExactly<MissingCulturesArgumentException>();
        }

        [Fact]
        public void Cannot_add_culture_confirugation_with_more_than_one_default_cultures()
        {
            IServiceCollection services;
            Dictionary<CultureInfo, bool> cultures;

            services = new FakeServiceCollection();
            cultures = new Dictionary<CultureInfo, bool>();
            cultures[new CultureInfo("en-US")] = true;
            cultures[new CultureInfo("en-GB")] = false;
            cultures[new CultureInfo("es-ES")] = true;

            Action action = () => services.AddLocalizationsCultureConfiguration(cultures);

            action.Should().ThrowExactly<MissingDefaultCultureArgumentException>();
        }

        [Fact]
        public void Cannot_add_culture_confirugation_without_any_default_culture()
        {
            IServiceCollection services;
            Dictionary<CultureInfo, bool> cultures;

            services = new FakeServiceCollection();
            cultures = new Dictionary<CultureInfo, bool>();
            cultures[new CultureInfo("en-US")] = false;
            cultures[new CultureInfo("es-ES")] = false;

            Action action = () => services.AddLocalizationsCultureConfiguration(cultures);

            action.Should().ThrowExactly<MissingDefaultCultureArgumentException>();
        }

        [Fact]
        public void Can_add_culture_confirugation_with_only_one_culture()
        {
            IServiceCollection services;
            Dictionary<CultureInfo, bool> cultures;

            services = new FakeServiceCollection();
            cultures = new Dictionary<CultureInfo, bool>();
            cultures[new CultureInfo("en-US")] = false;

            services.AddLocalizationsCultureConfiguration(cultures);

            services.Count.Should().Be(1);
        }

        [Fact]
        public void Can_add_culture_confirugation_with_only_one_culture_and_the_configuration_instance_has_right_default_and_cultures_properties()
        {
            IServiceCollection services;
            Dictionary<CultureInfo, bool> cultures;
            ServiceProvider provider;
            ILocalizationsCultureConfiguration configuration;

            services = new FakeServiceCollection();
            cultures = new Dictionary<CultureInfo, bool>();
            cultures[new CultureInfo("en-US")] = false;

            services.AddLocalizationsCultureConfiguration(cultures);

            provider = services.BuildServiceProvider();
            configuration = provider.GetService<ILocalizationsCultureConfiguration>();

            configuration.Should().NotBeNull();

            configuration.Default.Name.Should().Be("en-US");
            configuration.Cultures.Should().HaveCount(1);
            configuration.Cultures.Single().Name.Should().Be("en-US");
        }

        [Fact]
        public void Can_add_culture_confirugation()
        {
            IServiceCollection services;
            Dictionary<CultureInfo, bool> cultures;
            ServiceProvider provider;
            ILocalizationsCultureConfiguration configuration;

            services = new FakeServiceCollection();
            cultures = new Dictionary<CultureInfo, bool>();
            cultures[new CultureInfo("en-US")] = false;
            cultures[new CultureInfo("es-ES")] = true;

            services.AddLocalizationsCultureConfiguration(cultures);

            provider = services.BuildServiceProvider();
            configuration = provider.GetService<ILocalizationsCultureConfiguration>();

            configuration.Should().NotBeNull();

            configuration.Default.Name.Should().Be("es-ES");
            configuration.Cultures.Should().HaveCount(2);
            configuration.Cultures.Single(c => c.Name == "es-ES");
            configuration.Cultures.Single(c => c.Name == "en-US");
        }
    }
}
