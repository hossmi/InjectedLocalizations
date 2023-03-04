using System;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class ILocalizationsNotDerivedTypeError : AbstractTypeError
    {
        public ILocalizationsNotDerivedTypeError(Type type) : base($"Type does not inherits from {Usage.LocalizationsInterfaceType.Name}.", type) { }
    }
}
