using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class ReturnTypeMemberError : AbstractMemberError
    {
        public ReturnTypeMemberError(MemberInfo member)
            : base($@"Return type must be {typeof(string).Name}.", member) { }
    }
}
