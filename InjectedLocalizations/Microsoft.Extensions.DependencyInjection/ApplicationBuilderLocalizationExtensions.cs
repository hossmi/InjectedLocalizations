using InjectedLocalizations;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderLocalizationExtensions
    {
        public static IApplicationBuilder AddStaticLocalizations(this IApplicationBuilder app)
        {
            Localizations.ServiceProvider = app.ApplicationServices;
            return app;
        }
    }
}
