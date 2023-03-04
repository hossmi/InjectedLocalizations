using InjectedLocalizations.Building;

namespace InjectedLocalizations.Providers
{
    public interface ILocalizationsProvider
    {
        void TryBuildLocalizationFor(ILocalizationRequest request, ICodeBuilder builder);

        bool CanIssueToWriters { get; }
        void RegisterWriter(ILocalizationsProvider provider);

        bool CanWriteFromIssuers { get; }
        void Write(ILocalizationResponse response);
    }
}