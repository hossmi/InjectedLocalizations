using System.Collections.Generic;
using System.Reflection;

namespace InjectedLocalizations.MemberParsing
{
    public interface IParsedMember : IReadOnlyCollection<IToken>
    {
        MemberInfo Member { get; }
    }
}
