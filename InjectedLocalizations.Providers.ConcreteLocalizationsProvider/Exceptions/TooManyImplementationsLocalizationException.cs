using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    public class TooManyImplementationsLocalizationException : LocalizationException
    {
        public TooManyImplementationsLocalizationException(IEnumerable<string> errors)
            : base($"Only one concrete class can exists for a derived {nameof(ILocalizations)} and {nameof(CultureInfo)} pair.")
        {
            this.Errors = errors;
        }

        protected TooManyImplementationsLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IEnumerable<string> Errors { get; }
    }
}