using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class MultipleAttributesLocalizationException : ParsingLocalizationException
    {
        public MultipleAttributesLocalizationException(MemberInfo member)
            : base($"Cannot use muliple localization attributes at same member.")
        {
            this.Member = member;
        }

        protected MultipleAttributesLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MemberInfo Member { get; }
    }
}