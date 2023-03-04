using System;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class DeindentationUnderflowLocalizationException : LocalizationException
    {
        public DeindentationUnderflowLocalizationException() : base($"Codebuilder indentation cannot be less than zero.")
        {
        }

        protected DeindentationUnderflowLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}