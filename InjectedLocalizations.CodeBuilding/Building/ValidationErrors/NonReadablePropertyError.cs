using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class NonReadablePropertyError : AbstractPropertyError
    {
        public NonReadablePropertyError(PropertyInfo property) : base($@"The property is not readable.", property) { }
    }
}
