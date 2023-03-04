using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Building;
using InjectedLocalizations.Exceptions;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Providers
{
    public abstract class AbstractLocalizationsProvider : ILocalizationsProvider
    {
        private readonly HashSet<ILocalizationsProvider> writers;

        public AbstractLocalizationsProvider()
        {
            this.writers = new HashSet<ILocalizationsProvider>();
        }

        public abstract bool CanIssueToWriters { get; }
        public abstract bool CanWriteFromIssuers { get; }

        public void RegisterWriter(ILocalizationsProvider provider)
        {
            provider.ShouldBeNotNull(nameof(provider));

            if (ReferenceEquals(provider, this))
                throw new ArgumentException($"Attempt to register provider to itself.");

            if (!this.CanIssueToWriters)
                throw new NotIssuerProviderLocalizationException(this);

            if (!provider.CanWriteFromIssuers)
                throw new NotWritableProviderLocalizationException(provider);

            this.writers.Add(provider);
        }

        public void TryBuildLocalizationFor(ILocalizationRequest request, ICodeBuilder builder)
        {
            ILocalizationResponse response;
            string randomChunk;
            IReadOnlyDictionary<MemberInfo, string> localizations;

            request.ShouldBeNotNull(nameof(request));
            builder.ShouldBeNotNull(nameof(builder));

            localizations = GetLocalizationsOrNull(request);

            if (localizations == null)
                return;

            randomChunk = Guid
                .NewGuid()
                .ToString("N");

            response = new PrvResponse
            {
                Request = request,
                Members = localizations,
            };

            builder.BuildSpecificCultureLocalizationClass(response, randomChunk);

            if (response.Members.Any())
                foreach (ILocalizationsProvider writer in this.writers)
                    writer.Write(response);
        }

        public abstract void Write(ILocalizationResponse response);

        protected abstract IReadOnlyDictionary<MemberInfo, string> GetLocalizationsOrNull(ILocalizationRequest request);

        private class PrvResponse : ILocalizationResponse
        {
            public IReadOnlyDictionary<MemberInfo, string> Members { get; set; }
            public ILocalizationRequest Request { get; set; }
        }
    }
}
