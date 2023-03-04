using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class GenericParametersMethodError : AbstractMethodError
    {
        public GenericParametersMethodError(MethodInfo method)
            : base($"Method is generic", method) { }
    }
}
