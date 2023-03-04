using System.Globalization;
using FluentAssertions;
using InjectedLocalizations.Abstractions;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace InjectedLocalizations.Tests
{
    public class AbstractLocalizationsTests
    {
        [Fact]
        public void Can_create_enUS_instance()
        {
            IServiceCollection services;
            ServiceProvider serviceProvider;
            IDictionary<CultureInfo, Type> types;
            ILocalizationsCultureConfiguration localizationsCultureConfiguration;
            ICurrentCultureProvider currentCultureProvider;
            object instance;
            CultureInfo culture;

            culture = new CultureInfo("en-US");
            types = new Dictionary<CultureInfo, Type>();
            types[culture] = typeof(EnUsSomeLocalizations);

            localizationsCultureConfiguration = Substitute.For<ILocalizationsCultureConfiguration>();
            localizationsCultureConfiguration.Default.Returns(culture);
            localizationsCultureConfiguration.Cultures.Returns(new[] { culture });

            currentCultureProvider = Substitute.For<ICurrentCultureProvider>();
            currentCultureProvider
                .TryGetCurrent(out Arg.Any<CultureInfo>())
                .Returns(x =>
                {
                    x[0] = culture;
                    return true;
                });

            services = new FakeServiceCollection();
            services.AddSingleton(localizationsCultureConfiguration);
            services.AddSingleton(currentCultureProvider);
            serviceProvider = services.BuildServiceProvider();

            instance = AbstractLocalizationsExtensions.CreateInstance(serviceProvider, types);

            instance.Should().NotBeNull();
            instance.Should().BeOfType<EnUsSomeLocalizations>();
        }

        [Fact]
        public void Can_create_requested_english_instance()
        {
            IServiceCollection services;
            ServiceProvider serviceProvider;
            IDictionary<CultureInfo, Type> types;
            ILocalizationsCultureConfiguration localizationsCultureConfiguration;
            ICurrentCultureProvider currentCultureProvider;
            object instance;
            CultureInfo englishCulture, spanishCulture;

            englishCulture = new CultureInfo("en-US");
            spanishCulture = new CultureInfo("es-ES");
            types = new Dictionary<CultureInfo, Type>();
            types[englishCulture] = typeof(EnUsSomeLocalizations);
            types[spanishCulture] = typeof(EsEsSomeLocalizations);

            localizationsCultureConfiguration = Substitute.For<ILocalizationsCultureConfiguration>();
            localizationsCultureConfiguration.Default.Returns(englishCulture);
            localizationsCultureConfiguration.Cultures.Returns(new[] { spanishCulture, englishCulture });

            currentCultureProvider = Substitute.For<ICurrentCultureProvider>();
            currentCultureProvider
                .TryGetCurrent(out Arg.Any<CultureInfo>())
                .Returns(x =>
                {
                    x[0] = englishCulture;
                    return true;
                });

            services = new FakeServiceCollection();
            services.AddSingleton(localizationsCultureConfiguration);
            services.AddSingleton(currentCultureProvider);
            serviceProvider = services.BuildServiceProvider();

            instance = AbstractLocalizationsExtensions.CreateInstance(serviceProvider, types);

            instance.Should().NotBeNull();
            instance.Should().BeOfType<EnUsSomeLocalizations>();
        }

        [Fact]
        public void Can_create_requested_spanish_instance()
        {
            IServiceCollection services;
            ServiceProvider serviceProvider;
            IDictionary<CultureInfo, Type> types;
            ILocalizationsCultureConfiguration localizationsCultureConfiguration;
            ICurrentCultureProvider currentCultureProvider;
            object instance;
            CultureInfo englishCulture, spanishCulture;

            englishCulture = new CultureInfo("en-US");
            spanishCulture = new CultureInfo("es-ES");
            types = new Dictionary<CultureInfo, Type>();
            types[englishCulture] = typeof(EnUsSomeLocalizations);
            types[spanishCulture] = typeof(EsEsSomeLocalizations);

            localizationsCultureConfiguration = Substitute.For<ILocalizationsCultureConfiguration>();
            localizationsCultureConfiguration.Default.Returns(englishCulture);
            localizationsCultureConfiguration.Cultures.Returns(new[] { spanishCulture, englishCulture });

            currentCultureProvider = Substitute.For<ICurrentCultureProvider>();
            currentCultureProvider
                .TryGetCurrent(out Arg.Any<CultureInfo>())
                .Returns(x =>
                {
                    x[0] = spanishCulture;
                    return true;
                });

            services = new FakeServiceCollection();
            services.AddSingleton(localizationsCultureConfiguration);
            services.AddSingleton(currentCultureProvider);
            serviceProvider = services.BuildServiceProvider();

            instance = AbstractLocalizationsExtensions.CreateInstance(serviceProvider, types);

            instance.Should().NotBeNull();
            instance.Should().BeOfType<EsEsSomeLocalizations>();
        }

        [Fact]
        public void Can_create_default_english_instance()
        {
            IServiceCollection services;
            ServiceProvider serviceProvider;
            IDictionary<CultureInfo, Type> types;
            ILocalizationsCultureConfiguration localizationsCultureConfiguration;
            ICurrentCultureProvider currentCultureProvider;
            object instance;
            CultureInfo englishCulture, spanishCulture;

            englishCulture = new CultureInfo("en-US");
            spanishCulture = new CultureInfo("es-ES");
            types = new Dictionary<CultureInfo, Type>();
            types[englishCulture] = typeof(EnUsSomeLocalizations);
            types[spanishCulture] = typeof(EsEsSomeLocalizations);

            localizationsCultureConfiguration = Substitute.For<ILocalizationsCultureConfiguration>();
            localizationsCultureConfiguration.Default.Returns(englishCulture);
            localizationsCultureConfiguration.Cultures.Returns(new[] { spanishCulture, englishCulture });

            currentCultureProvider = Substitute.For<ICurrentCultureProvider>();
            currentCultureProvider
                .TryGetCurrent(out Arg.Any<CultureInfo>())
                .Returns(x =>
                {
                    x[0] = null;
                    return false;
                });

            services = new FakeServiceCollection();
            services.AddSingleton(localizationsCultureConfiguration);
            services.AddSingleton(currentCultureProvider);
            serviceProvider = services.BuildServiceProvider();

            instance = AbstractLocalizationsExtensions.CreateInstance(serviceProvider, types);

            instance.Should().NotBeNull();
            instance.Should().BeOfType<EnUsSomeLocalizations>();
        }

        [Fact]
        public void Cannot_create_instance_if_type_is_not_present_at_the_dictionary_1()
        {
            IServiceCollection services;
            ServiceProvider serviceProvider;
            IDictionary<CultureInfo, Type> types;
            ILocalizationsCultureConfiguration localizationsCultureConfiguration;
            ICurrentCultureProvider currentCultureProvider;
            object instance;
            CultureInfo englishCulture, spanishCulture;
            Action act;

            englishCulture = new CultureInfo("en-US");
            spanishCulture = new CultureInfo("es-ES");
            types = new Dictionary<CultureInfo, Type>();
            types[spanishCulture] = typeof(EsEsSomeLocalizations);

            localizationsCultureConfiguration = Substitute.For<ILocalizationsCultureConfiguration>();
            localizationsCultureConfiguration.Default.Returns(englishCulture);
            localizationsCultureConfiguration.Cultures.Returns(new[] { spanishCulture, englishCulture });

            currentCultureProvider = Substitute.For<ICurrentCultureProvider>();
            currentCultureProvider
                .TryGetCurrent(out Arg.Any<CultureInfo>())
                .Returns(x =>
                {
                    x[0] = null;
                    return false;
                });

            services = new FakeServiceCollection();
            services.AddSingleton(localizationsCultureConfiguration);
            services.AddSingleton(currentCultureProvider);
            serviceProvider = services.BuildServiceProvider();

            act = () => instance = AbstractLocalizationsExtensions.CreateInstance(serviceProvider, types);

            act.Should().ThrowExactly<NotFoundConcreteTypeForSelectedCultureLocalizationException>()
                .Which
                .Culture.Should().Be(englishCulture);
        }

        [Fact]
        public void Cannot_create_instance_if_type_is_not_present_at_the_dictionary_2()
        {
            IServiceCollection services;
            ServiceProvider serviceProvider;
            IDictionary<CultureInfo, Type> types;
            ILocalizationsCultureConfiguration localizationsCultureConfiguration;
            ICurrentCultureProvider currentCultureProvider;
            object instance;
            CultureInfo englishCulture, spanishCulture, japaneseCulture;
            Action act;

            englishCulture = new CultureInfo("en-US");
            spanishCulture = new CultureInfo("es-ES");
            japaneseCulture = new CultureInfo("ja-JP");
            types = new Dictionary<CultureInfo, Type>();
            types[spanishCulture] = typeof(EsEsSomeLocalizations);
            types[englishCulture] = typeof(EnUsSomeLocalizations);

            localizationsCultureConfiguration = Substitute.For<ILocalizationsCultureConfiguration>();
            localizationsCultureConfiguration.Default.Returns(englishCulture);
            localizationsCultureConfiguration.Cultures.Returns(new[] { spanishCulture, englishCulture });

            currentCultureProvider = Substitute.For<ICurrentCultureProvider>();
            currentCultureProvider
                .TryGetCurrent(out Arg.Any<CultureInfo>())
                .Returns(x =>
                {
                    x[0] = japaneseCulture;
                    return true;
                });

            services = new FakeServiceCollection();
            services.AddSingleton(localizationsCultureConfiguration);
            services.AddSingleton(currentCultureProvider);
            serviceProvider = services.BuildServiceProvider();

            act = () => instance = AbstractLocalizationsExtensions.CreateInstance(serviceProvider, types);

            act.Should().ThrowExactly<NotFoundConcreteTypeForSelectedCultureLocalizationException>()
                .Which
                .Culture.Should().Be(japaneseCulture);
        }

        [Fact]
        public void Cannot_create_instance_if_type_has_no_default_constructor()
        {
            IServiceCollection services;
            ServiceProvider serviceProvider;
            IDictionary<CultureInfo, Type> types;
            ILocalizationsCultureConfiguration localizationsCultureConfiguration;
            ICurrentCultureProvider currentCultureProvider;
            object instance;
            CultureInfo englishCulture, spanishCulture, japaneseCulture;
            Action act;

            englishCulture = new CultureInfo("en-US");
            spanishCulture = new CultureInfo("es-ES");
            japaneseCulture = new CultureInfo("ja-JP");
            types = new Dictionary<CultureInfo, Type>();
            types[spanishCulture] = typeof(EsEsSomeLocalizations);
            types[englishCulture] = typeof(EnUsSomeLocalizations);
            types[japaneseCulture] = typeof(ParametrizedCultureSomeLocalizations);

            localizationsCultureConfiguration = Substitute.For<ILocalizationsCultureConfiguration>();
            localizationsCultureConfiguration.Default.Returns(englishCulture);
            localizationsCultureConfiguration.Cultures.Returns(new[] { spanishCulture, englishCulture, japaneseCulture });

            currentCultureProvider = Substitute.For<ICurrentCultureProvider>();
            currentCultureProvider
                .TryGetCurrent(out Arg.Any<CultureInfo>())
                .Returns(x =>
                {
                    x[0] = japaneseCulture;
                    return true;
                });

            services = new FakeServiceCollection();
            services.AddSingleton(localizationsCultureConfiguration);
            services.AddSingleton(currentCultureProvider);
            serviceProvider = services.BuildServiceProvider();

            act = () => instance = AbstractLocalizationsExtensions.CreateInstance(serviceProvider, types);

            act.Should().ThrowExactly<ConstructorNotFoundLocalizationException>()
                .Which
                .Type.Should().Be(typeof(ParametrizedCultureSomeLocalizations));
        }
    }
}
