using System.Reflection;

namespace InjectedLocalizations.MemberParsing
{
    public interface IMemberParser
    {
        IParsedMember Parse(MemberInfo member);
    }
}
