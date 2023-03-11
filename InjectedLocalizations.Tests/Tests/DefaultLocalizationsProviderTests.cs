using System.Globalization;
using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Building.ValidationErrors;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace InjectedLocalizations.Tests
{
    public class DefaultLocalizationsProviderTests
    {
        [Fact]
        public void Default_provider_mades_use_of_the_builder()
        {
            ILocalizationRequest request;
            FakeCodeBuilder builder;
            DefaultLocalizationsProvider provider;

            request = Build.Request<IEmptyLocalizations>("en-US");
            builder = new FakeCodeBuilder();
            provider = new DefaultLocalizationsProvider();

            provider.TryBuildLocalizationFor(request, builder);

            builder.Buffer.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Default_provider_does_not_invoke_subscribed_writers_because_interface_has_no_members()
        {
            ILocalizationRequest request;
            FakeCodeBuilder builder;
            ILocalizationsProvider writer1, writer2;
            DefaultLocalizationsProvider provider;

            request = Build.Request<IEmptyLocalizations>("en-US");
            builder = new FakeCodeBuilder();
            writer1 = Substitute.For<ILocalizationsProvider>();
            writer1.CanWriteFromIssuers.Returns(true);
            writer2 = Substitute.For<ILocalizationsProvider>();
            writer2.CanWriteFromIssuers.Returns(true);

            provider = new DefaultLocalizationsProvider();
            provider.RegisterWriter(writer1);
            provider.RegisterWriter(writer2);

            provider.TryBuildLocalizationFor(request, builder);

            writer1.DidNotReceive().Write(Arg.Any<ILocalizationResponse>());
            writer2.DidNotReceive().Write(Arg.Any<ILocalizationResponse>());
        }

        [Fact]
        public void Default_provider_invokes_subscribed_writes()
        {
            ILocalizationRequest request;
            FakeCodeBuilder builder;
            ILocalizationsProvider writer1, writer2;
            DefaultLocalizationsProvider provider;

            request = Build.Request<IValidLocalizations>("en-US");
            builder = new FakeCodeBuilder();
            writer1 = Substitute.For<ILocalizationsProvider>();
            writer1.CanWriteFromIssuers.Returns(true);
            writer2 = Substitute.For<ILocalizationsProvider>();
            writer2.CanWriteFromIssuers.Returns(true);

            provider = new DefaultLocalizationsProvider();
            provider.RegisterWriter(writer1);
            provider.RegisterWriter(writer2);

            provider.TryBuildLocalizationFor(request, builder);

            writer1.Received(1).Write(Arg.Any<ILocalizationResponse>());
            writer2.Received(1).Write(Arg.Any<ILocalizationResponse>());
        }

        [Fact]
        public void Can_build_class_for_IEmptyLocalizations()
        {
            FakeCodeBuilder builder;
            IImplementationType implementation;
            string chunk;
            ILocalizationResponse response;

            response = Build.Response<IEmptyLocalizations>("en-US");
            builder = new FakeCodeBuilder(" ");
            chunk = "pepe";

            builder.BuildSpecificCultureLocalizationClass(response, chunk);

            builder.Implementations.Should().HaveCount(1);
            implementation = builder.Implementations.Single();
            implementation.Culture.Should().Be(response.Request.Culture);
            implementation.InterfaceType.Should().Be(response.Request.InterfaceType);
            implementation.ImplementationTypeName.Should().Be(response.Request.BuildSpecificCultureClassName(chunk));

            builder.References.Should().HaveCount(0);

            builder.Buffer.Should().Be(
                $"internal class EnglishUnitedStates_{chunk}_EmptyLocalizations " +
                    $": InjectedLocalizations.Models.IEmptyLocalizations " +
                    $"{{ " +
                        $"public System.Globalization.CultureInfo Culture {{ get; }} " +
                            $"= new System.Globalization.CultureInfo(\"en-US\");  " +
                    $"}} ");
        }

        [Fact]
        public void Can_build_class_for_IValidLocalizations()
        {
            FakeCodeBuilder builder;
            IImplementationType implementation;
            string chunk;
            ILocalizationResponse response;

            response = Build
                .Response<IValidLocalizations>("en-US")
                .WithMember(nameof(IValidLocalizations.Some_sample_property), "Some sample property")
                .WithMember(nameof(IValidLocalizations.That_tree_has_0_tasty_apples), "That tree has {applesCount} tasty apples")
                .WithMember(nameof(IValidLocalizations.The_user_0_with_code_1_is_forbiden), "The user {userName} with code {userId} is forbiden");

            builder = new FakeCodeBuilder(" ");
            chunk = "pepe";

            builder.BuildSpecificCultureLocalizationClass(response, chunk);

            builder.Implementations.Should().HaveCount(1);
            implementation = builder.Implementations.Single();
            implementation.Culture.Should().Be(response.Request.Culture);
            implementation.InterfaceType.Should().Be(response.Request.InterfaceType);
            implementation.ImplementationTypeName.Should().Be(response.Request.BuildSpecificCultureClassName(chunk));

            builder.References.Should().HaveCount(0);

            builder.Buffer.Should().Be(
                $"internal class EnglishUnitedStates_{chunk}_ValidLocalizations " +
                    $": InjectedLocalizations.Models.IValidLocalizations " +
                    $"{{ " +
                        $"public System.String Some_sample_property " +
                        $"{{ " +
                            $"get {{ return $@\"Some sample property\"; }} " +
                        $"}}  " +

                        $"public System.String That_tree_has_0_tasty_apples ( System.Int32 applesCount ) " +
                        $"{{ " +
                            $"return $@\"That tree has {{applesCount}} tasty apples\"; " +
                        $"}}  " +

                        $"public System.String The_user_0_with_code_1_is_forbiden ( System.String userName , System.Guid userId ) " +
                        $"{{ " +
                            $"return $@\"The user {{userName}} with code {{userId}} is forbiden\"; " +
                        $"}}  " +

                        $"public System.Globalization.CultureInfo Culture {{ get; }} = new System.Globalization.CultureInfo(\"en-US\");  " +
                    $"}} ");
        }

        [Fact]
        public void Can_generate_good_class_name()
        {
            ILocalizationRequest request;
            string className;

            request = Build.Request<IEmptyLocalizations>("en-US");

            className = request.BuildSpecificCultureClassName("pepe");

            className.Should().Be("EnglishUnitedStates_pepe_EmptyLocalizations");
        }

        [Fact]
        public void Can_get_en_US_culture_safe_english_name()
        {
            CultureInfo culture;
            string name;

            culture = new CultureInfo("en-US");

            name = culture.GetEnglishSafeClassName();

            name.Should().Be("EnglishUnitedStates");
        }

        [Fact]
        public void Can_get_es_ES_culture_safe_english_name()
        {
            CultureInfo culture;
            string name;

            culture = new CultureInfo("es-ES");

            name = culture.GetEnglishSafeClassName();

            name.Should().Be("SpanishSpainInternationalSort");
        }

        [Fact]
        public void Can_get_ja_JP_culture_safe_english_name()
        {
            CultureInfo culture;
            string name;

            culture = new CultureInfo("ja-JP");

            name = culture.GetEnglishSafeClassName();

            name.Should().Be("JapaneseJapan");
        }

        [Fact]
        public void Can_get_ar_SA_culture_safe_english_name()
        {
            CultureInfo culture;
            string name;

            culture = new CultureInfo("ar-SA");

            name = culture.GetEnglishSafeClassName();

            name.Should().Be("ArabicSaudiArabia");
        }

        [Fact]
        public void Can_get_ko_KR_culture_safe_english_name()
        {
            CultureInfo culture;
            string name;

            culture = new CultureInfo("ko-KR");

            name = culture.GetEnglishSafeClassName();

            name.Should().Be("KoreanKorea");
        }

        [Fact]
        public void Can_get_zh_CN_culture_safe_english_name()
        {
            CultureInfo culture;
            string name;

            culture = new CultureInfo("zh-CN");

            name = culture.GetEnglishSafeClassName();

            name.Should().Be("ChineseSimplifiedChina");
        }

        [Fact]
        public void Can_append_property()
        {
            FakeCodeBuilder builder;
            PropertyInfo property;

            builder = new FakeCodeBuilder(" ");
            property = typeof(IValidLocalizations)
                .GetProperty(nameof(IValidLocalizations.Some_sample_property));

            builder.AppendProperty(property, "pepe");

            builder.Buffer.Should().Be(@"public System.String Some_sample_property { get { return $@""pepe""; } }  ");
        }

        [Fact]
        public void Can_generate_culture_property()
        {
            FakeCodeBuilder builder;
            PropertyInfo property;
            CultureInfo culture;

            builder = new FakeCodeBuilder(" ");
            culture = new CultureInfo("es-ES");
            property = typeof(ILocalizations).GetProperty(nameof(ILocalizations.Culture));

            builder.AppendCultureProperty(property, culture);

            builder.Buffer
                .Should()
                .Be($"public System.Globalization.CultureInfo Culture {{ get; }} = new System.Globalization.CultureInfo(\"{culture.Name}\");  ");
        }

        [Fact]
        public void Can_append_method()
        {
            FakeCodeBuilder builder;
            MethodInfo method;

            builder = new FakeCodeBuilder(" ");
            method = typeof(IValidLocalizations)
                .GetMethod(nameof(IValidLocalizations.The_user_0_with_code_1_is_forbiden));

            builder.AppendMethod(method, "pepe");

            builder.Buffer.Should().Be($"public System.String The_user_0_with_code_1_is_forbiden " +
                $"( System.String userName , System.Guid userId ) " +
                $"{{ " +
                $"return $@\"pepe\"; " +
                $"}}  ");
        }

        [Fact]
        public void Can_append_members_of_IEmptyLocalizations()
        {
            FakeCodeBuilder builder;
            ILocalizationResponse response;

            builder = new FakeCodeBuilder(" ");
            response = Build.Response<IEmptyLocalizations>("en-US");

            builder.AppendSpecificClassMembers(response);

            builder.Buffer.Should().Be($"public System.Globalization.CultureInfo Culture " +
                $"{{ get; }} = new System.Globalization.CultureInfo(\"en-US\");  ");
        }

        [Fact]
        public void Can_append_members_of_IValidLocalizations()
        {
            FakeCodeBuilder builder;
            ILocalizationResponse response;

            builder = new FakeCodeBuilder(" ");
            response = Build
                .Response<IValidLocalizations>("en-US")
                .WithMember(nameof(IValidLocalizations.Some_sample_property), "Some sample property")
                .WithMember(nameof(IValidLocalizations.That_tree_has_0_tasty_apples), "That tree has {applesCount} tasty apples");

            builder.AppendSpecificClassMembers(response);

            builder.Buffer.Should().Be($"public System.String Some_sample_property {{ get {{ return $@\"Some sample property\"; }} }}  " +
                $"public System.String That_tree_has_0_tasty_apples ( System.Int32 applesCount ) {{ return $@\"That tree has {{applesCount}} tasty apples\"; }}  " +
                $"public System.Globalization.CultureInfo Culture {{ get; }} = new System.Globalization.CultureInfo(\"en-US\");  ");
        }

        [Fact]
        public void Cannot_append_constructor_members()
        {
            FakeCodeBuilder builder;
            ILocalizationResponse response;
            Action act;
            MemberInfo wrongMember;

            builder = new FakeCodeBuilder(" ");

            wrongMember = typeof(FakeCodeBuilder)
                .GetConstructors()
                .First();

            response = Build
                .Response<IValidLocalizations>("en-US")
                .WithMember(nameof(IValidLocalizations.Some_sample_property), "Some sample property")
                .WithMember(wrongMember, "Achtun!");

            act = () => builder.AppendSpecificClassMembers(response);

            act.Should().ThrowExactly<InvalidInterfacesLocalizationException>()
                .Which
                .ValidationErrors.First().Should().BeOfType<NotValidMemberError>()
                .Which
                .Member.Should().Be(wrongMember);
        }

        [Fact]
        public void Cannot_append_members_of_type_which_does_not_inherits_from_ILocalizations()
        {
            FakeCodeBuilder builder;
            FakeLocalizationResponse response;
            Action act;

            builder = new FakeCodeBuilder(" ");
            response = new FakeLocalizationResponse
            {
                Request = new FakeLocalizationRequest
                {
                    Culture = new CultureInfo("en-US"),
                    InterfaceType = typeof(ISample),
                },
            };

            act = () => builder.AppendSpecificClassMembers(response);

            act.Should().ThrowExactly<InvalidInterfacesLocalizationException>()
                .Which
                .ValidationErrors.First().Should().BeOfType<ILocalizationsNotDerivedTypeError>()
                .Which
                .Type.Should().Be<ISample>();
        }
    }
}
