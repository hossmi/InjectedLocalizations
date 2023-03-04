using System;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class GenericTypeError : AbstractTypeError
    {
        public GenericTypeError(Type type) : base($"Type is generic.", type) { }
    }
}
