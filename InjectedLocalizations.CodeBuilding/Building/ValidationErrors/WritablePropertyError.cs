using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class WritablePropertyError : AbstractPropertyError
    {
        public WritablePropertyError(PropertyInfo property)
            : base($@"The property is writable.", property) { }
    }
}
