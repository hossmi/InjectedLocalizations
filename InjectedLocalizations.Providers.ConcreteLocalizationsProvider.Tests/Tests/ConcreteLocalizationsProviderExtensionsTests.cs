using System.Globalization;
using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Extensions;
using InjectedLocalizations.Models;
using NSubstitute;
using Xunit;

namespace InjectedLocalizations.Tests
{
    public class ConcreteLocalizationsProviderExtensionsTests
    {
        [Fact]
        public void Can_build_dictionary_from_empty_assembly_configuration()
        {
            ILocalizationsAssembliesConfiguration assembliesConfiguration;
            IDictionary<Type, IDictionary<CultureInfo, Type>> dictionary;

            assembliesConfiguration = Substitute.For<ILocalizationsAssembliesConfiguration>();
            assembliesConfiguration.Assemblies.Returns(new Assembly[0]);

            dictionary = assembliesConfiguration.BuildMasterDictionary();

            dictionary.Should().NotBeNull();
            dictionary.Should().HaveCount(0);
        }

        [Fact]
        public void Can_build_dictionary_from_current_assembly()
        {
            ILocalizationsAssembliesConfiguration assembliesConfiguration;
            IDictionary<Type, IDictionary<CultureInfo, Type>> dictionary;

            assembliesConfiguration = Substitute.For<ILocalizationsAssembliesConfiguration>();

            assembliesConfiguration
                .Assemblies
                .Returns(new[]
                {
                    typeof(EnglishEmptyLocalizations).Assembly,
                });

            dictionary = assembliesConfiguration.BuildMasterDictionary();

            dictionary.Should().NotBeNull();
            dictionary.Should().HaveCount(1);
            dictionary[typeof(IEmptyLocalizations)].Should().HaveCount(2);
            dictionary[typeof(IEmptyLocalizations)][new CultureInfo("es-ES")].Should().Be(typeof(SpanishEmptyLocalizations));
            dictionary[typeof(IEmptyLocalizations)][new CultureInfo("en-US")].Should().Be(typeof(EnglishEmptyLocalizations));
        }

        [Fact]
        public void Can_get_culture()
        {
            CultureInfo culture;

            culture = typeof(SpanishEmptyLocalizations).GetLocalizationsCulture();

            culture.Should().NotBeNull();
            culture.Name.Should().Be("es-ES");
        }

        [Fact]
        public void Cannot_get_culture_if_type_does_not_have_right_constructor()
        {
            CultureInfo culture;

            Action act = () => culture = typeof(IEmptyLocalizations).GetLocalizationsCulture();

            act.Should().Throw<ConstructorNotFoundLocalizationException>();
        }

        [Fact]
        public void Can_create_masterdictionary_from_empty_collection()
        {
            InterfaceCultureImplementationRow[] rows;
            IDictionary<Type, IDictionary<CultureInfo, Type>> dictionary;

            rows = new InterfaceCultureImplementationRow[0];

            dictionary = rows.AsMasterDictionary();

            dictionary.Should().NotBeNull();
            dictionary.Should().HaveCount(0);
        }

        [Fact]
        public void Can_create_masterdictionary_from_single_item()
        {
            InterfaceCultureImplementationRow[] rows;
            IDictionary<Type, IDictionary<CultureInfo, Type>> dictionary;
            CultureInfo culture;

            culture = new CultureInfo("en-US");
            rows = new[]
            {
                new InterfaceCultureImplementationRow
                {
                    Interface = typeof(IEmptyLocalizations),
                    Culture = culture,
                    Implementation = typeof(EnglishEmptyLocalizations),
                },
            };

            dictionary = rows.AsMasterDictionary();

            dictionary.Should().NotBeNull();
            dictionary.Should().HaveCount(1);
            dictionary[typeof(IEmptyLocalizations)].Should().HaveCount(1);
            dictionary[typeof(IEmptyLocalizations)][culture].Should().Be(typeof(EnglishEmptyLocalizations));
        }

        [Fact]
        public void Can_create_masterdictionary_from_two_items()
        {
            InterfaceCultureImplementationRow[] rows;
            IDictionary<Type, IDictionary<CultureInfo, Type>> dictionary;
            CultureInfo englishCulture, spanishCulture;

            englishCulture = new CultureInfo("en-US");
            spanishCulture = new CultureInfo("es-ES");

            rows = new[]
            {
                new InterfaceCultureImplementationRow
                {
                    Interface = typeof(IEmptyLocalizations),
                    Culture = englishCulture,
                    Implementation = typeof(EnglishEmptyLocalizations),
                },
                new InterfaceCultureImplementationRow
                {
                    Interface = typeof(IEmptyLocalizations),
                    Culture = spanishCulture,
                    Implementation = typeof(SpanishEmptyLocalizations),
                },
            };

            dictionary = rows.AsMasterDictionary();

            dictionary.Should().NotBeNull();
            dictionary.Should().HaveCount(1);
            dictionary[typeof(IEmptyLocalizations)].Should().HaveCount(2);
            dictionary[typeof(IEmptyLocalizations)][englishCulture].Should().Be(typeof(EnglishEmptyLocalizations));
            dictionary[typeof(IEmptyLocalizations)][spanishCulture].Should().Be(typeof(SpanishEmptyLocalizations));
        }

        [Fact]
        public void Cannot_create_masterdictionary_from_repeated_items()
        {
            InterfaceCultureImplementationRow[] rows;
            IDictionary<Type, IDictionary<CultureInfo, Type>> dictionary;
            CultureInfo englishCulture;

            englishCulture = new CultureInfo("en-US");

            rows = new[]
            {
                new InterfaceCultureImplementationRow
                {
                    Interface = typeof(IEmptyLocalizations),
                    Culture = englishCulture,
                    Implementation = typeof(EnglishEmptyLocalizations),
                },
                new InterfaceCultureImplementationRow
                {
                    Interface = typeof(IEmptyLocalizations),
                    Culture = englishCulture,
                    Implementation = typeof(EnglishEmptyLocalizations),
                },
            };

            Action act = () => dictionary = rows.AsMasterDictionary();

            act.Should().ThrowExactly<TooManyImplementationsLocalizationException>();
        }
    }
}
