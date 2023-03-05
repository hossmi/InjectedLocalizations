using System;
using System.Globalization;
using JimenaTools.Extensions.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace InjectedLocalizations.Providers
{
    public class HttpContextCurrentCultureProvider : ICurrentCultureProvider
    {
        private readonly IServiceProvider serviceProvider;

        public HttpContextCurrentCultureProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider.ShouldBeNotNull(nameof(serviceProvider));
        }

        public bool TryGetCurrent(out CultureInfo culture)
        {
            culture = this.serviceProvider
                .GetService<IHttpContextAccessor>()?
                .HttpContext?
                .Features
                .Get<IRequestCultureFeature>()?
                .RequestCulture?
                .Culture;

            return culture != null;
        }
    }
}
