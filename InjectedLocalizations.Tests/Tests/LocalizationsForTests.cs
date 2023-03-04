using FluentAssertions;
using InjectedLocalizations.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace InjectedLocalizations.Tests
{
    public class LocalizationsForTests
    {
        [Fact]
        public void Can_get_localization_for_class_from_static_entry_point()
        {
            IServiceProvider serviceProvider;
            IValidLocalizations localizations, localizationsFake;
            IApplicationBuilder app;
            Guid guid;
            string name;

            name = "pepe";
            guid = Guid.NewGuid();
            localizationsFake = Substitute.For<IValidLocalizations>();
            localizationsFake
                .The_user_0_with_code_1_is_forbiden(Arg.Is(name), Arg.Is(guid))
                .Returns($"The user pepe with code {guid} is forbiden");

            serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider
                .GetService(Arg.Is(typeof(IValidLocalizations)))
                .Returns(localizationsFake);

            app = Substitute.For<IApplicationBuilder>();
            app.ApplicationServices.Returns(serviceProvider);

            app.AddStaticLocalizations();
            localizations = Localizations.For<IValidLocalizations>();

            localizations.Should().NotBeNull();
            localizations
                .The_user_0_with_code_1_is_forbiden("pepe", guid)
                .Should()
                .Be($"The user pepe with code {guid} is forbiden");
        }
    }
}
