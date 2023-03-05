using System;
using System.Globalization;
using FluentAssertions;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace InjectedLocalizations.Tests
{
    public class HttpContextCurrentCultureProviderTests
    {
        [Fact]
        public void HttpContextCurrentCultureProvider_returns_culture()
        {
            FakeServiceCollection services;
            HttpContextCurrentCultureProvider provider;
            IServiceProvider serviceProvider;
            IHttpContextAccessor contextAccessor;
            FakeHttpContext httpContext;
            IRequestCultureFeature requestCultureFeature;
            bool success;

            requestCultureFeature = Substitute.For<IRequestCultureFeature>();
            requestCultureFeature.RequestCulture.Returns(new RequestCulture("en-US"));

            httpContext = new FakeHttpContext();
            httpContext.SetFeature(requestCultureFeature);

            contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Returns(httpContext);

            services = new FakeServiceCollection();
            services.AddSingleton(contextAccessor);
            serviceProvider = services.BuildServiceProvider();

            provider = new HttpContextCurrentCultureProvider(serviceProvider);

            success = provider.TryGetCurrent(out CultureInfo culture);

            success.Should().BeTrue();
            culture.Should().NotBeNull();
            culture.Name.Should().Be("en-US");
        }

        [Fact]
        public void Cannot_retrieve_culture_without_requestCultureFeature()
        {
            FakeServiceCollection services;
            HttpContextCurrentCultureProvider provider;
            ServiceProvider serviceProvider;
            IHttpContextAccessor contextAccessor;
            FakeHttpContext httpContext;
            bool success;

            httpContext = new FakeHttpContext();

            contextAccessor = Substitute.For<IHttpContextAccessor>();
            contextAccessor.HttpContext.Returns(httpContext);

            services = new FakeServiceCollection();
            services.AddSingleton(contextAccessor);
            serviceProvider = services.BuildServiceProvider();

            provider = new HttpContextCurrentCultureProvider(serviceProvider);

            success = provider.TryGetCurrent(out CultureInfo culture);

            success.Should().BeFalse();
            culture.Should().BeNull();
        }

        [Fact]
        public void Cannot_retrieve_culture_without_context_accessor()
        {
            FakeServiceCollection services;
            HttpContextCurrentCultureProvider provider;
            ServiceProvider serviceProvider;
            bool success;

            services = new FakeServiceCollection();
            serviceProvider = services.BuildServiceProvider();

            provider = new HttpContextCurrentCultureProvider(serviceProvider);

            success = provider.TryGetCurrent(out CultureInfo culture);

            success.Should().BeFalse();
            culture.Should().BeNull();
        }
    }
}
