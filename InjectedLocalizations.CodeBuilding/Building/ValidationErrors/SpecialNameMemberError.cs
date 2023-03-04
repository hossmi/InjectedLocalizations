using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class SpecialNameMemberError : AbstractMemberError
    {
        public SpecialNameMemberError(MemberInfo member) : base("Property has special name", member) { }
    }
}
