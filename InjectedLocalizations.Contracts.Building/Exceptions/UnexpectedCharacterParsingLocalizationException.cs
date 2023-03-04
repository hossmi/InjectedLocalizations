using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    public class UnexpectedCharacterParsingLocalizationException : ParsingLocalizationException
    {
        public UnexpectedCharacterParsingLocalizationException(IEnumerator<char> enumerator, string buffer)
            : base($"Unexpected or unsupported character")
        {
            this.Enumerator = enumerator;
            this.Buffer = buffer;
        }

        protected UnexpectedCharacterParsingLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IEnumerator<char> Enumerator { get; }
        public string Buffer { get; }
    }
}