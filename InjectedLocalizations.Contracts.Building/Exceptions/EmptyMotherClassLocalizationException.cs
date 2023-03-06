using System;
using System.Runtime.Serialization;
using InjectedLocalizations.Building;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class EmptyMotherClassLocalizationException : LocalizationException
    {
        public EmptyMotherClassLocalizationException(ICodeBuilder builder, Type interfaceType)
            : base($"Cannot construct a mother class without child implementations.")
        {
            this.Builder = builder;
            this.InterfaceType = interfaceType;
        }

        protected EmptyMotherClassLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ICodeBuilder Builder { get; }
        public Type InterfaceType { get; }
    }
}