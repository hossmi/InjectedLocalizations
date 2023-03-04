using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Building;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.MemberParsing;
using JimenaTools.Extensions.Strings;

namespace InjectedLocalizations.Providers
{
    public class DefaultLocalizationsProvider : AbstractLocalizationsProvider
    {
        public override bool CanIssueToWriters => true;
        public override bool CanWriteFromIssuers => false;

        public override void Write(ILocalizationResponse response) => throw new NotWritableProviderLocalizationException(this);

        protected override IReadOnlyDictionary<MemberInfo, string> GetLocalizationsOrNull(ILocalizationRequest request)
        {
            return request
                .InterfaceType
                .GetMethodsAndProperties()
                .Where(m => !m.IsILocalizationsCultureProperty())
                .Select(MemberParserExtensions.AsParsedMember)
                .Select(parsed => new
                {
                    parsed.Member,
                    Value = parsed
                        .AsInterpolatedString()
                        .AsJoined(),
                })
                .ToDictionary(m => m.Member, m => m.Value);
        }
    }
}
