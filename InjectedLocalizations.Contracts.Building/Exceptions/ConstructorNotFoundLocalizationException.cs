using System;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class ConstructorNotFoundLocalizationException : LocalizationException
    {
        public ConstructorNotFoundLocalizationException(Type type, Type[] requiredParameters)
            : base($"Type does not have constructor with required parameters")
        {
            this.Type = type;
            this.RequiredParameters = requiredParameters;
        }

        protected ConstructorNotFoundLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Type Type { get; }
        public Type[] RequiredParameters { get; }
    }
}