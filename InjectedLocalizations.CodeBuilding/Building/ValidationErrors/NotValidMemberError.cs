using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class NotValidMemberError : AbstractMemberError
    {
        public NotValidMemberError(MemberInfo member) : base($"Member is not a method neither a property.", member) { }
    }
}
