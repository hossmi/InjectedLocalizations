using System.Reflection;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public abstract class AbstractPropertyError : AbstractError
    {
        public AbstractPropertyError(string message, PropertyInfo property) : base(message, property.DeclaringType)
        {
            this.Property = property;
        }

        public PropertyInfo Property { get; }

        public override string ToString()
        {
            return $"{this.Message}: {this.Type.Name}.{this.Property.Name} {{ {(this.Property.CanRead ? "get;" : "")}{(this.Property.CanWrite ? "set;" : "")} }}";
        }
    }
}
