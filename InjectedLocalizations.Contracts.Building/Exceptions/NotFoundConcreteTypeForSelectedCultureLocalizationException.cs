using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class NotFoundConcreteTypeForSelectedCultureLocalizationException : LocalizationException
    {
        public NotFoundConcreteTypeForSelectedCultureLocalizationException(CultureInfo culture) : base("The requested culture was not found.")
        {
            this.Culture = culture;
        }

        protected NotFoundConcreteTypeForSelectedCultureLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CultureInfo Culture { get; }
    }
}