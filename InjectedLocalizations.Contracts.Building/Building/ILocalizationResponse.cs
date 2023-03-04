using System.Collections.Generic;
using System.Reflection;

namespace InjectedLocalizations.Building
{
    public interface ILocalizationResponse
    {
        ILocalizationRequest Request { get; }
        IReadOnlyDictionary<MemberInfo, string> Members { get; }
    }
}
