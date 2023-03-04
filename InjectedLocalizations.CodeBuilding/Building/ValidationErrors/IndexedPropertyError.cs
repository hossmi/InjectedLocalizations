using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public class IndexedPropertyError : AbstractPropertyError
    {
        public IndexedPropertyError(PropertyInfo property) : base($@"Is an index property", property) { }
    }
}
