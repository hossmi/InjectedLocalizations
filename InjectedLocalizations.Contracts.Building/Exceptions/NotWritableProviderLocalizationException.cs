using System;
using System.Runtime.Serialization;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class NotWritableProviderLocalizationException : LocalizationException
    {
        public NotWritableProviderLocalizationException(ILocalizationsProvider provider) : base("The provider is read only.")
        {
        }

        protected NotWritableProviderLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}