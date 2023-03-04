using System.Globalization;
using FluentAssertions;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace InjectedLocalizations.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddInterfacedLocalizations_fails_without_adding_cultures()
        {
            FakeServiceCollection services;
            Action addInterfacedLocalizationsCall;

            services = new FakeServiceCollection();
            addInterfacedLocalizationsCall = () => services
                .AddInterfacedLocalizations(options => options.SetAssemblyOf<IEmptyLocalizations>());

            addInterfacedLocalizationsCall.Should().ThrowExactly<MissingCulturesArgumentException>();
        }

        [Fact]
        public void AddInterfacedLocalizations_adds_default_services()
        {
            FakeServiceCollection services;
            ServiceDescriptor descriptor;

            services = new FakeServiceCollection();
            services.AddInterfacedLocalizations(options =>
            {
                options.SetCulture(new CultureInfo("en-US"));
                options.SetAssemblyOf<IEmptyLocalizations>();
            });

            services.Should().HaveCount(14);
            descriptor = services.Single(d => d.ServiceType == typeof(IAssemblyBuilder));
            descriptor = services.Single(d => d.ServiceType == typeof(ICurrentCultureProvider));
            descriptor = services.Single(d => d.ServiceType == typeof(ILocalizationsCacheService));
            descriptor = services.Single(d => d.ServiceType == typeof(ILocalizationsCultureConfiguration));
            descriptor = services.Single(d => d.ServiceType == typeof(ILocalizationsAssembliesConfiguration));
            descriptor = services.Single(d => d.ServiceType == typeof(ILocalizationsProviderConfiguration));
            descriptor = services.Single(d => d.ServiceType == typeof(DefaultLocalizationsProvider));
        }
    }
}
