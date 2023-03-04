using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Models;

namespace InjectedLocalizations.Tests
{
    public class IsILocalizationsCulturePropertyTests
    {
        [Fact]
        public void Can_check_ILocalizations_Culture_property()
        {
            typeof(EmptyLocalizations)
                .GetProperty(nameof(ILocalizations.Culture))
                .IsILocalizationsCultureProperty()
                .Should().BeTrue();
        }

        [Fact]
        public void String_Lenght_property_is_not_ILocalizations_Culture_property()
        {
            typeof(string)
                .GetProperty(nameof(string.Length))
                .IsILocalizationsCultureProperty()
                .Should().BeFalse();
        }

        [Fact]
        public void FakeCulturePropertyClass_does_not_have_ILocalizations_Culture_property()
        {
            typeof(FakeCulturePropertyClass)
                .GetProperty(nameof(FakeCulturePropertyClass.Culture))
                .IsILocalizationsCultureProperty()
                .Should().BeFalse();
        }

        [Fact]
        public void Can_check_ILocalizations_Culture_member()
        {
            typeof(EmptyLocalizations)
                .GetMember(nameof(ILocalizations.Culture))
                .Single()
                .IsILocalizationsCultureProperty()
                .Should().BeTrue();
        }

        [Fact]
        public void String_Lenght_property_is_not_ILocalizations_Culture_member()
        {
            typeof(string)
                .GetMember(nameof(string.Length))
                .Single()
                .IsILocalizationsCultureProperty()
                .Should().BeFalse();
        }

        [Fact]
        public void FakeCulturePropertyClass_does_not_have_ILocalizations_Culture_member()
        {
            typeof(FakeCulturePropertyClass)
                .GetMember(nameof(FakeCulturePropertyClass.Culture))
                .Single()
                .IsILocalizationsCultureProperty()
                .Should().BeFalse();
        }
    }
}
