using System.Reflection;
using InjectedLocalizations.Building;

namespace InjectedLocalizations.Models
{
    public class FakeLocalizationResponse : ILocalizationResponse
    {
        public FakeLocalizationResponse()
        {
            this.Members = new Dictionary<MemberInfo, string>();
        }

        public ILocalizationRequest Request { get; set; }
        public Dictionary<MemberInfo, string> Members { get; }
        IReadOnlyDictionary<MemberInfo, string> ILocalizationResponse.Members => this.Members;
    }
}
