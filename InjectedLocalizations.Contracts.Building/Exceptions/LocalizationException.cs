using System;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class LocalizationException : Exception
    {
        public LocalizationException(string message) : base(message)
        {
        }

        public LocalizationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}