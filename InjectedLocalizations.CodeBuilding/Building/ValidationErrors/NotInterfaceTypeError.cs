using System;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class NotInterfaceTypeError : AbstractTypeError
    {
        public NotInterfaceTypeError(Type type) : base($"Type is not an interface.", type) { }
    }
}
