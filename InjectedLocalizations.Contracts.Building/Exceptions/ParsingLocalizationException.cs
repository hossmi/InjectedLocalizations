using System;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    public class ParsingLocalizationException : LocalizationException
    {
        public ParsingLocalizationException(string message) : base(message)
        {
        }

        public ParsingLocalizationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParsingLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}