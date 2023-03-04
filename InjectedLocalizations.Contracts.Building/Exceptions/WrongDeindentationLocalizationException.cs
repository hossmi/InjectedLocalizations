using System;
using System.Runtime.Serialization;
using InjectedLocalizations.Building;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class WrongDeindentationLocalizationException : LocalizationException
    {
        public WrongDeindentationLocalizationException(IIndentation token)
            : base("Error trying to deindent with wrong token.")
        {
            this.Token = token;
        }

        protected WrongDeindentationLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IIndentation Token { get; }
    }
}