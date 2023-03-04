using System.Globalization;
using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Tests
{
    public class CodeBuilderExtensionsTests
    {
        [Fact]
        public void Cannot_build_source_without_cultures()
        {
            FakeCodeBuilder codeBuilder;
            ILocalizationsProviderConfiguration providerConfiguration;
            ILocalizationsCultureConfiguration cultureConfiguration;
            Action buildSourceCall;

            codeBuilder = new FakeCodeBuilder();
            providerConfiguration = Build.LocalizationsProviderConfiguration();
            cultureConfiguration = Build.LocalizationsCultureConfiguration("en-US");

            buildSourceCall = () => codeBuilder.BuildSource(typeof(IEmptyLocalizations), providerConfiguration, cultureConfiguration);

            buildSourceCall.Should().ThrowExactly<MissingCulturesArgumentException>();
        }

        [Fact]
        public void Cannot_build_source_without_providers()
        {
            FakeCodeBuilder codeBuilder;
            ILocalizationsProviderConfiguration providerConfiguration;
            ILocalizationsCultureConfiguration cultureConfiguration;
            Action buildSourceCall;

            codeBuilder = new FakeCodeBuilder();
            providerConfiguration = Build.LocalizationsProviderConfiguration();
            cultureConfiguration = Build.LocalizationsCultureConfiguration("en-US", "es-ES");

            buildSourceCall = () => codeBuilder.BuildSource(typeof(IEmptyLocalizations), providerConfiguration, cultureConfiguration);

            buildSourceCall.Should().ThrowExactly<SpecificTypeNotGeneratedLocalizationException>();
        }

        [Fact]
        public void Can_build_source()
        {
            FakeCodeBuilder builder;
            ILocalizationsProviderConfiguration providerConfiguration;
            ILocalizationsCultureConfiguration cultureConfiguration;

            builder = new FakeCodeBuilder();
            providerConfiguration = Build.LocalizationsProviderConfiguration(Build.Provider((r, b) =>
            {
                b.Append($"/*{r.Culture.Name}*/");
            }));
            cultureConfiguration = Build.LocalizationsCultureConfiguration("en-US", "es-ES", "en-GB");

            builder.BuildSource(typeof(IValidLocalizations), providerConfiguration, cultureConfiguration);

            builder.Buffer.Should().Be("namespace Utilities.Globalization.Generated{/*es-ES*//*en-GB*/}");
            builder.Implementations.Should().HaveCount(0);
        }

        [Fact]
        public void Can_filter_candidate_interfaces()
        {
            IReadOnlyCollection<Type> types, filteredTypes;

            types = new[]
            {
                typeof(IEmptyLocalizations),
                typeof(IValidLocalizations),
            };

            filteredTypes = types.FilterCandidateInterfaces().ToList();

            filteredTypes.Should().BeEquivalentTo(types);
        }

        [Fact]
        public void Can_filter_candidate_interfaces_excluding_non_valid_types()
        {
            IReadOnlyCollection<Type> types, expectedTypes, filteredTypes;

            types = new[]
            {
                typeof(IEmptyLocalizations),
                typeof(object),
                typeof(IValidLocalizations),
                typeof(FakeCodeBuilder),
                typeof(ICodeBuilder),
            };

            expectedTypes = new[]
            {
                typeof(IEmptyLocalizations),
                typeof(IValidLocalizations),
            };

            filteredTypes = types.FilterCandidateInterfaces().ToList();

            filteredTypes.Should().BeEquivalentTo(expectedTypes);
        }

        [Fact]
        public void Providers_do_not_add_code_for_no_request()
        {
            FakeCodeBuilder builder;
            IEnumerable<ILocalizationRequest> requests;
            IEnumerable<ILocalizationsProvider> providers;

            builder = new FakeCodeBuilder();
            requests = new ILocalizationRequest[0];
            providers = new ILocalizationsProvider[0];

            builder.BuildSpecificCultureLocalizationClasses(requests, providers);

            builder.Buffer.Length.Should().Be(0);
            builder.Implementations.Should().BeEmpty();
        }

        [Fact]
        public void Provider_receives_one_call_for_one_request()
        {
            FakeCodeBuilder builder;
            IEnumerable<ILocalizationRequest> requests;
            IEnumerable<ILocalizationsProvider> providers;
            int providerCalls;

            builder = new FakeCodeBuilder();
            providerCalls = 0;

            requests = new[]
            {
                Build.Request<IEmptyLocalizations>("en-US"),
            };

            providers = new[]
            {
                Build.Provider((r, b) =>
                {
                    providerCalls++;
                    b.Append("x");
                }),
            };

            builder.BuildSpecificCultureLocalizationClasses(requests, providers);

            builder.Buffer.Should().Be("x");
            builder.Implementations.Should().BeEmpty();
            providerCalls.Should().Be(1);
        }

        [Fact]
        public void Provider_receives_one_call_for_each_requests()
        {
            FakeCodeBuilder builder;
            IEnumerable<ILocalizationRequest> requests;
            IEnumerable<ILocalizationsProvider> providers;
            int providerCalls;
            Action<ILocalizationRequest, ICodeBuilder> onTryBuild;

            builder = new FakeCodeBuilder();
            providerCalls = 0;

            requests = new[]
            {
                Build.Request<IEmptyLocalizations>("en-US"),
                Build.Request<IEmptyLocalizations>("es-ES"),
            };

            onTryBuild = (r, b) =>
            {
                providerCalls++;
                b.Append(",");
            };

            providers = new[]
            {
                Build.Provider(onTryBuild),
            };

            builder.BuildSpecificCultureLocalizationClasses(requests, providers);

            builder.Buffer.Should().Be(",,");
            builder.Implementations.Should().BeEmpty();
            providerCalls.Should().Be(2);
        }

        [Fact]
        public void The_first_provider_does_not_add_code_but_the_second_do()
        {
            FakeCodeBuilder builder;
            IEnumerable<ILocalizationRequest> requests;
            IEnumerable<ILocalizationsProvider> providers;
            int providerCalls;

            builder = new FakeCodeBuilder();
            providerCalls = 0;

            requests = new[]
            {
                Build.Request<IEmptyLocalizations>("en-US"),
            };

            providers = new[]
            {
                Build.Provider((r, b) =>
                {
                    ++providerCalls;
                }),
                Build.Provider((r, b) =>
                {
                    ++providerCalls;
                    b.Append(providerCalls.ToString());
                }),
            };

            builder.BuildSpecificCultureLocalizationClasses(requests, providers);

            builder.Buffer.Should().Be("2");
            builder.Implementations.Should().BeEmpty();
            providerCalls.Should().Be(2);
        }

        [Fact]
        public void Two_providers_with_two_request_generates_four_calls()
        {
            FakeCodeBuilder builder;
            IEnumerable<ILocalizationRequest> requests;
            IEnumerable<ILocalizationsProvider> providers;
            int providerCalls;

            builder = new FakeCodeBuilder();
            providerCalls = 0;

            requests = new[]
            {
                Build.Request<IEmptyLocalizations>("en-US"),
                Build.Request<IEmptyLocalizations>("es-ES"),
            };

            providers = new[]
            {
                Build.Provider((r, b) =>
                {
                    ++providerCalls;
                }),
                Build.Provider((r, b) =>
                {
                    ++providerCalls;
                    b.Append(providerCalls.ToString());
                }),
            };

            builder.BuildSpecificCultureLocalizationClasses(requests, providers);

            builder.Buffer.Should().Be("24");
            builder.Implementations.Should().BeEmpty();
            providerCalls.Should().Be(4);
        }

        [Fact]
        public void When_the_first_provider_produces_code_the_second_one_is_never_called()
        {
            FakeCodeBuilder builder;
            IEnumerable<ILocalizationRequest> requests;
            IEnumerable<ILocalizationsProvider> providers;
            int firstProviderCalls, secondProviderCalls;

            builder = new FakeCodeBuilder();
            firstProviderCalls = 0;
            secondProviderCalls = 0;

            requests = new[]
            {
                Build.Request<IEmptyLocalizations>("en-US"),
                Build.Request<IEmptyLocalizations>("es-ES"),
            };

            providers = new[]
            {
                Build.Provider((r, b) =>
                {
                    firstProviderCalls++;
                    b.Append(r.Culture.ThreeLetterISOLanguageName).Append(",");
                }),
                Build.Provider((r, b) => secondProviderCalls++),
            };

            builder.BuildSpecificCultureLocalizationClasses(requests, providers);

            builder.Buffer.Should().Be(@"eng,spa,");
            builder.Implementations.Should().BeEmpty();
            firstProviderCalls.Should().Be(2);
            secondProviderCalls.Should().Be(0);
        }

        [Fact]
        public void Cannot_build_specific_class_without_providers()
        {
            FakeCodeBuilder builder;
            IEnumerable<ILocalizationRequest> requests;
            IEnumerable<ILocalizationsProvider> providers;
            Action buildSpecificCultureLocalizationClassesCall;

            builder = new FakeCodeBuilder();

            requests = new[]
            {
                Build.Request<IEmptyLocalizations>("en-US"),
            };

            providers = new ILocalizationsProvider[0];

            buildSpecificCultureLocalizationClassesCall = () => builder.BuildSpecificCultureLocalizationClasses(requests, providers);

            buildSpecificCultureLocalizationClassesCall.Should().ThrowExactly<SpecificTypeNotGeneratedLocalizationException>();
        }

        [Fact]
        public void It_will_not_generate_mother_classes_in_absence_of_specific_classes()
        {
            FakeCodeBuilder builder;
            CultureInfo culture;

            builder = new FakeCodeBuilder();
            culture = new CultureInfo("en-US");

            builder.BuildMotherClasses(culture, () => "pepe");

            builder.Buffer.Should().BeEmpty();
            builder.Implementations.Should().BeEmpty();
            builder.References.Should().BeEmpty();
        }

        [Fact]
        public void Can_generate_one_mother_class_for_one_implemetation()
        {
            FakeCodeBuilder builder;
            CultureInfo culture;

            builder = new FakeCodeBuilder();
            culture = new CultureInfo("en-US");

            builder.SetImplementation(new FakeImplementationType
            {
                Culture = culture,
                ImplementationTypeName = typeof(EmptyLocalizations).FullName,
                InterfaceType = typeof(IEmptyLocalizations),
            });

            builder.BuildMotherClasses(culture, () => "pepe");

            builder.Buffer.Should().Be(
                "public class Mother_pepe_EmptyLocalizations " +
                    ": Utilities.Globalization.Implementations.AbstractLocalizations<Utilities.Globalization.Tests.RightModels.IEmptyLocalizations>" +
                    ", Utilities.Globalization.Tests.RightModels.IEmptyLocalizations" +
                "{" +
                    "private static readonly System.Collections.Generic.IDictionary<System.Globalization.CultureInfo,System.Type> typeDescriptors;" +
                    "private static readonly System.Globalization.CultureInfo defaultCulture;" +
                    "static Mother_pepe_EmptyLocalizations()" +
                    "{" +
                        "typeDescriptors = new System.Collections.Generic.Dictionary<System.Globalization.CultureInfo,System.Type>();" +
                        "typeDescriptors.Add(new System.Globalization.CultureInfo(\"en-US\"),typeof(Utilities.Globalization.Tests.Models.EmptyLocalizations));" +
                        "defaultCulture = new System.Globalization.CultureInfo(\"en-US\");" +
                    "}" +
                    "public Mother_pepe_EmptyLocalizations(System.IServiceProvider serviceProvider) : base(serviceProvider, typeDescriptors) " +
                    "{ " +
                    "}" +
                    $"public System.Globalization.CultureInfo Culture => base.instance.Culture;" +
                "}");
        }

        [Fact]
        public void Can_generate_one_mother_class_for_two_implemetations()
        {
            FakeCodeBuilder builder;
            CultureInfo culture;

            builder = new FakeCodeBuilder();
            culture = new CultureInfo("en-US");

            builder.SetImplementation(new FakeImplementationType
            {
                Culture = culture,
                ImplementationTypeName = typeof(EmptyLocalizations).FullName,
                InterfaceType = typeof(IEmptyLocalizations),
            });

            builder.SetImplementation(new FakeImplementationType
            {
                Culture = new CultureInfo("es-ES"),
                ImplementationTypeName = typeof(EmptyLocalizations).FullName,
                InterfaceType = typeof(IEmptyLocalizations),
            });

            builder.BuildMotherClasses(culture, () => "pepe");

            builder.Buffer.Should().Be(
                "public class Mother_pepe_EmptyLocalizations " +
                    ": Utilities.Globalization.Implementations.AbstractLocalizations<Utilities.Globalization.Tests.RightModels.IEmptyLocalizations>" +
                    ", Utilities.Globalization.Tests.RightModels.IEmptyLocalizations" +
                "{" +
                    "private static readonly System.Collections.Generic.IDictionary<System.Globalization.CultureInfo,System.Type> typeDescriptors;" +
                    "private static readonly System.Globalization.CultureInfo defaultCulture;" +
                    "static Mother_pepe_EmptyLocalizations()" +
                    "{" +
                        "typeDescriptors = new System.Collections.Generic.Dictionary<System.Globalization.CultureInfo,System.Type>();" +
                        "typeDescriptors.Add(new System.Globalization.CultureInfo(\"en-US\"),typeof(Utilities.Globalization.Tests.Models.EmptyLocalizations));" +
                        "typeDescriptors.Add(new System.Globalization.CultureInfo(\"es-ES\"),typeof(Utilities.Globalization.Tests.Models.EmptyLocalizations));" +
                        "defaultCulture = new System.Globalization.CultureInfo(\"en-US\");" +
                    "}" +
                    "public Mother_pepe_EmptyLocalizations(System.IServiceProvider serviceProvider) : base(serviceProvider, typeDescriptors) " +
                    "{ " +
                    "}" +
                    $"public System.Globalization.CultureInfo Culture => base.instance.Culture;" +
                "}");
        }

        [Fact]
        public void Can_generate_two_mother_classes_for_two_implemetation_groups()
        {
            FakeCodeBuilder builder;
            CultureInfo culture;

            builder = new FakeCodeBuilder();
            culture = new CultureInfo("en-US");

            builder.SetImplementation(new FakeImplementationType
            {
                Culture = new CultureInfo("es-ES"),
                ImplementationTypeName = typeof(decimal).FullName,
                InterfaceType = typeof(IValidLocalizations),
            });

            builder.SetImplementation(new FakeImplementationType
            {
                Culture = culture,
                ImplementationTypeName = typeof(DateTime).FullName,
                InterfaceType = typeof(IValidLocalizations),
            });

            builder.SetImplementation(new FakeImplementationType
            {
                Culture = new CultureInfo("es-ES"),
                ImplementationTypeName = typeof(EmptyLocalizations).FullName,
                InterfaceType = typeof(IEmptyLocalizations),
            });

            builder.SetImplementation(new FakeImplementationType
            {
                Culture = culture,
                ImplementationTypeName = typeof(EmptyLocalizations).FullName,
                InterfaceType = typeof(IEmptyLocalizations),
            });

            builder.BuildMotherClasses(culture, () => "pepe");

            builder.Buffer.Should().Be(
                "public class Mother_pepe_EmptyLocalizations " +
                    ": Utilities.Globalization.Implementations.AbstractLocalizations<Utilities.Globalization.Tests.RightModels.IEmptyLocalizations>" +
                    ", Utilities.Globalization.Tests.RightModels.IEmptyLocalizations" +
                "{" +
                    "private static readonly System.Collections.Generic.IDictionary<System.Globalization.CultureInfo,System.Type> typeDescriptors;" +
                    "private static readonly System.Globalization.CultureInfo defaultCulture;" +

                    "static Mother_pepe_EmptyLocalizations()" +
                    "{" +
                        "typeDescriptors = new System.Collections.Generic.Dictionary<System.Globalization.CultureInfo,System.Type>();" +
                        "typeDescriptors.Add(new System.Globalization.CultureInfo(\"en-US\"),typeof(Utilities.Globalization.Tests.Models.EmptyLocalizations));" +
                        "typeDescriptors.Add(new System.Globalization.CultureInfo(\"es-ES\"),typeof(Utilities.Globalization.Tests.Models.EmptyLocalizations));" +
                        "defaultCulture = new System.Globalization.CultureInfo(\"en-US\");" +
                    "}" +
                    "public Mother_pepe_EmptyLocalizations(System.IServiceProvider serviceProvider) : base(serviceProvider, typeDescriptors) { }" +
                    $"public System.Globalization.CultureInfo Culture => base.instance.Culture;" +
                "}" +

                "public class Mother_pepe_ValidLocalizations " +
                    ": Utilities.Globalization.Implementations.AbstractLocalizations<Utilities.Globalization.Tests.RightModels.IValidLocalizations>" +
                    ", Utilities.Globalization.Tests.RightModels.IValidLocalizations" +
                "{" +
                    "private static readonly System.Collections.Generic.IDictionary<System.Globalization.CultureInfo,System.Type> typeDescriptors;" +
                    "private static readonly System.Globalization.CultureInfo defaultCulture;" +
                    "static Mother_pepe_ValidLocalizations()" +
                    "{" +
                        "typeDescriptors = new System.Collections.Generic.Dictionary<System.Globalization.CultureInfo,System.Type>();" +
                        "typeDescriptors.Add(new System.Globalization.CultureInfo(\"en-US\"),typeof(System.DateTime));" +
                        "typeDescriptors.Add(new System.Globalization.CultureInfo(\"es-ES\"),typeof(System.Decimal));" +
                        "defaultCulture = new System.Globalization.CultureInfo(\"en-US\");" +
                    "}" +
                    "public Mother_pepe_ValidLocalizations(System.IServiceProvider serviceProvider) : base(serviceProvider, typeDescriptors) { }" +
                    $"public System.Globalization.CultureInfo Culture => base.instance.Culture;" +
                    "public System.String Some_sample_property => base.instance.Some_sample_property;" +
                    "public System.String That_tree_has_0_tasty_apples(System.Int32 applesCount)" +
                    "{" +
                        "return base.instance.That_tree_has_0_tasty_apples(applesCount);" +
                    "}" +
                    "public System.String The_user_0_with_code_1_is_forbiden(System.String userName, System.Guid userId)" +
                    "{" +
                        "return base.instance.The_user_0_with_code_1_is_forbiden(userName, userId);" +
                    "}" +
                "}");
        }

        [Fact]
        public void Try_to_build_mother_class_without_implementations_throws_exception()
        {
            FakeCodeBuilder builder;
            CultureInfo culture;
            Action buildMotherClassCall;

            builder = new FakeCodeBuilder();
            culture = new CultureInfo("en-US");
            buildMotherClassCall = () => builder.BuildMotherClass(typeof(IEmptyLocalizations), new IImplementation[0], culture, "pepe");

            buildMotherClassCall.Should().ThrowExactly<EmptyMotherClassLocalizationException>();
        }

        [Fact]
        public void Can_build_mother_class_for_IEmptyLocalizations()
        {
            FakeCodeBuilder builder;
            CultureInfo culture;
            IImplementation[] implementations;

            builder = new FakeCodeBuilder();
            culture = new CultureInfo("en-US");
            implementations = new IImplementation[]
            {
                Build.Implementation<decimal>("es-ES"),
            };

            builder.BuildMotherClass(typeof(IEmptyLocalizations), implementations, culture, "pepe");

            builder.Buffer.Should().Be(
                "public class Mother_pepe_EmptyLocalizations " +
                    ": Utilities.Globalization.Implementations.AbstractLocalizations<Utilities.Globalization.Tests.RightModels.IEmptyLocalizations>" +
                        ", Utilities.Globalization.Tests.RightModels.IEmptyLocalizations" +
                "{" +
                    "private static readonly System.Collections.Generic.IDictionary<System.Globalization.CultureInfo,System.Type> typeDescriptors;" +
                    "private static readonly System.Globalization.CultureInfo defaultCulture;" +

                    "static Mother_pepe_EmptyLocalizations()" +
                    "{" +
                        "typeDescriptors = new System.Collections.Generic.Dictionary<System.Globalization.CultureInfo,System.Type>();" +
                        "typeDescriptors.Add(new System.Globalization.CultureInfo(\"es-ES\"),typeof(System.Decimal));" +
                        "defaultCulture = new System.Globalization.CultureInfo(\"en-US\");" +
                    "}" +

                    "public Mother_pepe_EmptyLocalizations(System.IServiceProvider serviceProvider) " +
                        ": base(serviceProvider, typeDescriptors) { }" +
                    $"public System.Globalization.CultureInfo Culture => base.instance.Culture;" +
                "}");
        }

        [Fact]
        public void Can_build_mother_class_for_IValidLocalizations()
        {
            FakeCodeBuilder builder;
            CultureInfo culture;
            IImplementation[] implementations;

            builder = new FakeCodeBuilder();
            culture = new CultureInfo("en-US");
            implementations = new IImplementation[]
            {
                Build.Implementation<decimal>("es-ES"),
                Build.Implementation<decimal>("en-US"),
            };

            builder.BuildMotherClass(typeof(IValidLocalizations), implementations, culture, "pepe");

            builder.Buffer.Should().Be(
                "public class Mother_pepe_ValidLocalizations " +
                    ": Utilities.Globalization.Implementations.AbstractLocalizations<Utilities.Globalization.Tests.RightModels.IValidLocalizations>" +
                    ", Utilities.Globalization.Tests.RightModels.IValidLocalizations" +
                "{" +
                    "private static readonly System.Collections.Generic.IDictionary<System.Globalization.CultureInfo,System.Type> typeDescriptors;" +
                    "private static readonly System.Globalization.CultureInfo defaultCulture;" +

                    "static Mother_pepe_ValidLocalizations()" +
                    "{" +
                    "typeDescriptors = new System.Collections.Generic.Dictionary<System.Globalization.CultureInfo,System.Type>();" +
                    "typeDescriptors.Add(new System.Globalization.CultureInfo(\"es-ES\"),typeof(System.Decimal));" +
                    "typeDescriptors.Add(new System.Globalization.CultureInfo(\"en-US\"),typeof(System.Decimal));" +
                    "defaultCulture = new System.Globalization.CultureInfo(\"en-US\");" +
                    "}" +

                    "public Mother_pepe_ValidLocalizations(System.IServiceProvider serviceProvider) " +
                        ": base(serviceProvider, typeDescriptors) { }" +

                    $"public System.Globalization.CultureInfo Culture => base.instance.Culture;" +

                    "public System.String Some_sample_property => base.instance.Some_sample_property;" +

                    "public System.String That_tree_has_0_tasty_apples(System.Int32 applesCount)" +
                    "{" +
                        "return base.instance.That_tree_has_0_tasty_apples(applesCount);" +
                    "}" +

                    "public System.String The_user_0_with_code_1_is_forbiden(System.String userName, System.Guid userId)" +
                    "{" +
                        "return base.instance.The_user_0_with_code_1_is_forbiden(userName, userId);" +
                    "}" +
                "}");
        }

        [Fact]
        public void Can_get_dictionary_initialization_for_one_item()
        {
            FakeCodeBuilder builder;
            IEnumerator<IImplementation> enumerator;
            Action enumeratorCurrentCall;

            builder = new FakeCodeBuilder();

            enumerator = new IImplementation[]
                {
                    Build.Implementation<string>("en-US"),
                }
                .AsEnumerable()
                .GetEnumerator();

            enumerator.MoveNext();

            builder.AppendDictionaryInitializations(enumerator, "pepe", "x");

            builder.Buffer.Should().Be("pepe.Add(new x(\"en-US\"),typeof(System.String));");

            enumeratorCurrentCall = () => enumerator.Current.Should().BeNull();
            enumeratorCurrentCall.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Can_get_dictionary_initialization_for_two_items()
        {
            FakeCodeBuilder builder;
            IEnumerator<IImplementation> enumerator;
            Action enumeratorCurrentCall;

            builder = new FakeCodeBuilder();

            enumerator = new IImplementation[]
                {
                    Build.Implementation<string>("en-US"),
                    Build.Implementation<string>("es-ES"),
                }
                .AsEnumerable()
                .GetEnumerator();

            enumerator.MoveNext();

            builder.AppendDictionaryInitializations(enumerator, "pepe", "x");

            builder.Buffer.Should().Be(
                "pepe.Add(new x(\"en-US\"),typeof(System.String));" +
                "pepe.Add(new x(\"es-ES\"),typeof(System.String));");

            enumeratorCurrentCall = () => enumerator.Current.Should().BeNull();
            enumeratorCurrentCall.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Cannot_get_dictionary_initialization_without_item()
        {
            FakeCodeBuilder builder;
            IEnumerator<IImplementation> enumerator;
            Action appendDictionaryInitializationsCall;

            builder = new FakeCodeBuilder();

            enumerator = new IImplementation[0]
                .AsEnumerable()
                .GetEnumerator();

            enumerator.MoveNext();

            appendDictionaryInitializationsCall = () => builder.AppendDictionaryInitializations(enumerator, "pepe", "x");

            appendDictionaryInitializationsCall.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Can_AppendDictionaryInitialization()
        {
            FakeCodeBuilder builder;
            IImplementation implementation;

            builder = new FakeCodeBuilder();
            implementation = Build.Implementation<string>("en-US");

            builder.AppendDictionaryInitialization(implementation, "pepe", "x");

            builder.Buffer.Should().Be("pepe.Add(new x(\"en-US\"),typeof(System.String));");
        }

        [Fact]
        public void Can_AppendMotherMembers()
        {
            FakeCodeBuilder builder;

            builder = new FakeCodeBuilder();

            builder.AppendMotherMembers(typeof(IValidLocalizations));

            builder.Buffer.Should().Be(
                $"public System.Globalization.CultureInfo Culture => base.instance.Culture;" +

                "public System.String " +
                "Some_sample_property => base.instance.Some_sample_property;" +

                "public System.String " +
                "That_tree_has_0_tasty_apples(System.Int32 applesCount)" +
                "{" +
                    "return base.instance.That_tree_has_0_tasty_apples(applesCount);" +
                "}" +

                "public System.String " +
                "The_user_0_with_code_1_is_forbiden(System.String userName, System.Guid userId)" +
                "{" +
                    "return base.instance.The_user_0_with_code_1_is_forbiden(userName, userId);" +
                "}");
        }

        [Fact]
        public void Can_AppendMotherProperty()
        {
            FakeCodeBuilder builder;
            PropertyInfo property;

            builder = new FakeCodeBuilder();
            property = typeof(IValidLocalizations)
                .GetProperty(nameof(IValidLocalizations.Some_sample_property));

            builder.AppendMotherProperty(property);

            builder.Buffer.Should().Be("public System.String Some_sample_property => base.instance.Some_sample_property;");
        }

        [Fact]
        public void Can_AppendMotherMethod()
        {
            FakeCodeBuilder builder;
            MethodInfo method;

            builder = new FakeCodeBuilder();
            method = typeof(IValidLocalizations)
                .GetMethod(nameof(IValidLocalizations.The_user_0_with_code_1_is_forbiden));

            builder.AppendMotherMethod(method);

            builder.Buffer.Should().Be(
                "public System.String " +
                "The_user_0_with_code_1_is_forbiden(System.String userName, System.Guid userId)" +
                "{" +
                    "return base.instance.The_user_0_with_code_1_is_forbiden(userName, userId);" +
                "}");
        }
    }
}
