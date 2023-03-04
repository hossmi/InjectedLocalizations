using InjectedLocalizations.Building;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Models
{
    public class MinimalFakeLocalizationsProvider : ILocalizationsProvider
    {
        public bool CanIssueToWriters => false;
        public bool CanWriteFromIssuers => false;

        public void RegisterWriter(ILocalizationsProvider provider) => throw new NotIssuerProviderLocalizationException(this);

        public void TryBuildLocalizationFor(ILocalizationRequest request, ICodeBuilder builder) { }

        public void Write(ILocalizationResponse response) => throw new NotWritableProviderLocalizationException(this);
    }

}
