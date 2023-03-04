using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class NotSupportedReturnTypeLocalizationException : LocalizationException
    {
        public NotSupportedReturnTypeLocalizationException(MemberInfo member)
            : base($"Localization properties and methods must return {typeof(string).FullName}.")
        {
            this.Member = member;
        }

        protected NotSupportedReturnTypeLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MemberInfo Member { get; }
    }
}