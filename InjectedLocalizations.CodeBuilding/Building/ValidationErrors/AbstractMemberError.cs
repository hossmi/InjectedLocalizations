using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public abstract class AbstractMemberError : AbstractError
    {
        public AbstractMemberError(string message, MemberInfo member) : base(message, member.DeclaringType)
        {
            this.Member = member;
        }

        public MemberInfo Member { get; }

        public override string ToString()
        {
            string parenthesis;

            parenthesis = this.Member is MethodInfo ? "(...)" : "";
            return $"{this.Message}: {this.Type.Name}.{this.Member.Name}{parenthesis}";
        }
    }
}
