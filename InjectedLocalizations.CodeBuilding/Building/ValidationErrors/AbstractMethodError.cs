using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public abstract class AbstractMethodError : AbstractError
    {
        protected AbstractMethodError(string message, MethodInfo method) : base(message, method.DeclaringType)
        {
            this.Method = method;
        }

        public MethodInfo Method { get; }

        public override string ToString()
        {
            return $"{this.Message}: {this.Type.Name}.{this.Method.Name}(...)";
        }
    }
}
