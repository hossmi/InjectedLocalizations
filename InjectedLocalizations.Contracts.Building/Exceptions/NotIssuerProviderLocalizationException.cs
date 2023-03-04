using System;
using System.Runtime.Serialization;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class NotIssuerProviderLocalizationException : LocalizationException
    {
        public NotIssuerProviderLocalizationException(ILocalizationsProvider provider) : base("This localizations provider is not an issuer provider.")
        {
            this.Provider = provider;
        }

        protected NotIssuerProviderLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ILocalizationsProvider Provider { get; }
    }
}