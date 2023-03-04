using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class MissingDefaultCultureArgumentException : ArgumentException
    {
        private const string errorMessage = "In order to run localization system, you need to configure only one culture as default.";
        public MissingDefaultCultureArgumentException(IEnumerable<KeyValuePair<CultureInfo, bool>> cultures) : base(errorMessage)
        {
            this.Cultures = cultures.ToArray();
        }

        protected MissingDefaultCultureArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public KeyValuePair<CultureInfo, bool>[] Cultures { get; }
    }
}