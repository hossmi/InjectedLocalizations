using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using NSubstitute;
using Xunit;

namespace InjectedLocalizations.Tests
{
    public class ConcreteLocalizationsProviderTests
    {
        [Fact]
        public void TryBuildLocalizationFor_without_assemblies_does_not_found_implementations()
        {
            ILocalizationsAssembliesConfiguration assembliesConfiguration;
            ConcreteLocalizationsProvider provider;
            ILocalizationRequest request;
            FakeCodeBuilder builder;

            assembliesConfiguration = Substitute.For<ILocalizationsAssembliesConfiguration>();
            assembliesConfiguration.Assemblies.Returns(new Assembly[0]);
            request = Build.Request<IEmptyLocalizations>("es-ES");
            builder = new FakeCodeBuilder();

            provider = new ConcreteLocalizationsProvider(assembliesConfiguration);

            provider.TryBuildLocalizationFor(request, builder);

            builder.Buffer.Should().BeEmpty();
            builder.Implementations.Should().HaveCount(0);
        }

        [Fact]
        public void TryBuildLocalizationFor_finds_the_implementation()
        {
            ILocalizationsAssembliesConfiguration assembliesConfiguration;
            ConcreteLocalizationsProvider provider;
            ILocalizationRequest request;
            FakeCodeBuilder builder;
            IImplementationType implementation;

            assembliesConfiguration = Substitute.For<ILocalizationsAssembliesConfiguration>();
            assembliesConfiguration.Assemblies.Returns(new[] { typeof(EnglishEmptyLocalizations).Assembly });
            request = Build.Request<IEmptyLocalizations>("es-ES");
            builder = new FakeCodeBuilder();

            provider = new ConcreteLocalizationsProvider(assembliesConfiguration);

            provider.TryBuildLocalizationFor(request, builder);

            builder.Buffer.Should().BeEmpty();
            builder.Implementations.Should().HaveCount(1);

            implementation = builder.Implementations.Single();
            implementation.ImplementationTypeName.Should().Be(typeof(SpanishEmptyLocalizations).FullName);
        }
    }
}
