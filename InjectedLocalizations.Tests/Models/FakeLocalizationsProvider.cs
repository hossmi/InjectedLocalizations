using InjectedLocalizations.Building;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.Models
{
    public class FakeLocalizationsProvider : ILocalizationsProvider
    {
        public Action<ILocalizationsProvider> OnRegister { get; set; }
        public Action<ILocalizationRequest, ICodeBuilder> OnTryBuild { get; set; }
        public Action<ILocalizationResponse> OnWrite { get; set; }
        public bool CanIssueToWriters { get; set; }
        public bool CanWriteFromIssuers { get; set; }

        public void RegisterWriter(ILocalizationsProvider provider)
        {
            this.OnRegister?.Invoke(provider);
        }

        public void TryBuildLocalizationFor(ILocalizationRequest request, ICodeBuilder builder)
        {
            this.OnTryBuild?.Invoke(request, builder);
        }

        public void Write(ILocalizationResponse response)
        {
            this.OnWrite?.Invoke(response);
        }
    }
}
