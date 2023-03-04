using System;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class MissingCulturesArgumentException : ArgumentException
    {
        public MissingCulturesArgumentException(string paramName)
            : base("In order to run Localization system, you need to add at least one culture.", paramName)
        {
        }

        protected MissingCulturesArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}