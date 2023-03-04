using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.MemberParsing.Tokens;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.MemberParsing
{
    public static class MemberParserExtensions
    {
        public static IPrintableToken TryGetParameterToken(this IReadOnlyList<ParameterInfo> parameters, string number)
        {
            bool isInRange;

            isInRange = ushort.TryParse(number, out ushort index)
                && 0 <= index && index < parameters.Count;

            if (isInRange)
            {
                ParameterInfo parameter;

                parameter = parameters[index];
                return new ParameterToken(parameter);
            }

            return new NumberToken(number);
        }

        public static IParsedMember AsParsedMember(this MemberInfo member)
        {
            return GetParser(member)
                .Parse(member);
        }

        public static IMemberParser GetParser(MemberInfo member)
        {
            return member
                .GetCustomAttributes()
                .Where(a => a is AbstractParserAttribute)
                .Cast<AbstractParserAttribute>()
                .ShouldBeSingleOrDefault(() => new MultipleAttributesLocalizationException(member))?
                .Parser
                ?? new DefaultMemberParser();
        }
    }
}
