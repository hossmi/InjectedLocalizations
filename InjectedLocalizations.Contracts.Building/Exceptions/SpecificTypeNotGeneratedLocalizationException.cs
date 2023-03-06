using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using InjectedLocalizations.Building;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class SpecificTypeNotGeneratedLocalizationException : LocalizationException
    {
        public SpecificTypeNotGeneratedLocalizationException(ILocalizationRequest request, IEnumerable<ILocalizationsProvider> providers)
            : base($"No localization provider has generated specific culture localization type")
        {
            this.Request = request;
            this.Providers = providers.ToArray();
        }

        protected SpecificTypeNotGeneratedLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ILocalizationRequest Request { get; }
        public ILocalizationsProvider[] Providers { get; }
    }
}