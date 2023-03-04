using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class TypeLoadLocalizationException : LocalizationException
    {
        public TypeLoadLocalizationException(CultureInfo culture, Type type, Exception innerException)
            : base($"Failed creating localization instance", innerException)
        {
            this.Culture = culture;
            this.Type = type;
        }

        protected TypeLoadLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CultureInfo Culture { get; }
        public Type Type { get; }
    }
}