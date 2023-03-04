using System.Globalization;
using System.Reflection;
using InjectedLocalizations.Building;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.MemberParsing.Tokens;
using InjectedLocalizations.Providers;
using NSubstitute;

namespace InjectedLocalizations.Models
{
    public static class Build
    {
        public static IImplementation Implementation<T>(string culture)
        {
            IImplementation implementation;

            implementation = Substitute.For<IImplementation>();
            implementation.Culture.Returns(new CultureInfo(culture));
            implementation.ImplementationTypeName.Returns(typeof(T).FullName);

            return implementation;
        }

        public static ILocalizationsProviderConfiguration LocalizationsProviderConfiguration(params ILocalizationsProvider[] providers)
        {
            ILocalizationsProviderConfiguration configuration;

            configuration = Substitute.For<ILocalizationsProviderConfiguration>();
            configuration.Providers.Returns(providers);

            return configuration;
        }

        public static ILocalizationsCultureConfiguration LocalizationsCultureConfiguration(string defaultCulture, params string[] cultures)
        {
            ILocalizationsCultureConfiguration configuration;
            IReadOnlyCollection<CultureInfo> cultureInfos;

            cultureInfos = cultures
                .Select(c => new CultureInfo(c))
                .ToList();

            configuration = Substitute.For<ILocalizationsCultureConfiguration>();
            configuration.Cultures.Returns(cultureInfos);
            configuration.Default.Returns(new CultureInfo(defaultCulture));

            return configuration;
        }

        public static IParsedMember ParsedMember(MemberInfo member, params object[] tokensOrParameterIndexes)
        {
            IParsedMember parsedMember;
            IEnumerator<IToken> enumerator;

            parsedMember = Substitute.For<IParsedMember>();
            parsedMember.Member.Returns(member);
            parsedMember.Count.Returns(tokensOrParameterIndexes.Length);

            enumerator = GetTokens(member, tokensOrParameterIndexes).GetEnumerator();

            parsedMember
                .GetEnumerator()
                .Returns(enumerator);

            return parsedMember;
        }

        private static IEnumerable<IToken> GetTokens(MemberInfo member, object[] tokensOrParameterIndexes)
        {
            ParameterInfo[] parameters;

            if (member is MethodInfo method)
                parameters = method.GetParameters();
            else
                parameters = new ParameterInfo[0];

            foreach (object tokenOrIndex in tokensOrParameterIndexes)
            {
                if (tokenOrIndex is IToken token)
                    yield return token;

                else if (tokenOrIndex is int index)
                    yield return new ParameterToken(parameters[index]);

                else
                    throw new ArgumentException($"{nameof(tokenOrIndex)} must be {nameof(IToken)} or {nameof(Int32)}", nameof(tokensOrParameterIndexes));
            }
        }

        public static ILocalizationsProvider Provider(Action<ILocalizationRequest, ICodeBuilder> onTryBuild)
        {
            return new FakeLocalizationsProvider
            {
                OnTryBuild = onTryBuild,
            };
        }

        public static ILocalizationRequest Request<T>(string culture) where T : ILocalizations
        {
            return new FakeLocalizationRequest
            {
                Culture = new CultureInfo(culture),
                InterfaceType = typeof(T),
            };
        }

        public static FakeLocalizationResponse Response<T>(string culture) where T : ILocalizations
        {
            return new FakeLocalizationResponse
            {
                Request = Request<T>(culture),
            };
        }

        public static FakeLocalizationResponse WithMember(this FakeLocalizationResponse response, string memberName, string value)
        {
            MemberInfo member;

            member = response
                .Request
                .InterfaceType
                .GetMember(memberName)
                .Single();

            response.Members[member] = value;

            return response;
        }

        public static FakeLocalizationResponse WithMember(this FakeLocalizationResponse response, MemberInfo member, string value)
        {
            response.Members[member] = value;

            return response;
        }
    }
}
