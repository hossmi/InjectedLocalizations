using System.Collections.Generic;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.MemberParsing.Tokens;

namespace InjectedLocalizations.Providers
{
    public static class DefaultLocalizationsProviderExtensions
    {
        public static IEnumerable<string> AsInterpolatedString(this IParsedMember parsed)
        {
            foreach (IToken token in parsed)
            {
                if (!(token is IPrintableToken printable))
                    continue;

                if (printable is ParameterToken parameter)
                {
                    yield return "{";
                    yield return parameter.Value;
                    yield return "}";
                    continue;
                }

                yield return printable.Value;
            }
        }
    }
}
