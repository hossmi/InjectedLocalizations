using System.Collections;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Building.ValidationErrors;
using InjectedLocalizations.Models;

namespace InjectedLocalizations.Tests
{
    public class ValidateLocalizationInterfaceTests
    {
        [Fact]
        public void Can_filter_ILocalizations_derived_interface_without_errors()
        {
            typeof(IValidLocalizations)
                .ValidateLocalizationInterface()
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void Interface_with_index_properties_are_not_valid()
        {
            IReadOnlyCollection<IError> wrongMembers;

            wrongMembers = typeof(INotValidIndexedLocalizations)
                .ValidateLocalizationInterface()
                .ToList();

            wrongMembers.Should().HaveCount(2);
            wrongMembers.OfType<ReturnTypeMemberError>().Should().HaveCount(1);
            wrongMembers.OfType<IndexedPropertyError>().Should().HaveCount(1);
        }

        [Fact]
        public void Interface_methods_and_properties_must_return_string_or_IPluralString()
        {
            IReadOnlyCollection<IError> wrongMembers;

            wrongMembers = typeof(INotValidReturnTypeLocalizations)
                .ValidateLocalizationInterface()
                .ToList();

            wrongMembers.Should().HaveCount(2);
            wrongMembers.OfType<ReturnTypeMemberError>().Should().HaveCount(2);
        }

        [Fact]
        public void Type_must_be_interface()
        {
            IReadOnlyCollection<IError> wrongMembers;

            wrongMembers = typeof(ValidateLocalizationInterfaceTests)
                .ValidateLocalizationInterface()
                .ToList();

            wrongMembers.Should().HaveCount(1);
            wrongMembers.OfType<NotInterfaceTypeError>().Should().HaveCount(1);
        }

        [Fact]
        public void Type_must_inherit_from_ILocalizations()
        {
            IReadOnlyCollection<IError> wrongMembers;

            wrongMembers = typeof(IEnumerator)
                .ValidateLocalizationInterface()
                .ToList();

            wrongMembers.Should().HaveCount(1);
            wrongMembers.OfType<ILocalizationsNotDerivedTypeError>().Should().HaveCount(1);
        }

        [Fact]
        public void Type_must_be_non_generic_interface()
        {
            IReadOnlyCollection<IError> wrongMembers;

            wrongMembers = typeof(IGenericLocalizations<>)
                .ValidateLocalizationInterface()
                .ToList();

            wrongMembers.Should().HaveCount(1);
            wrongMembers.OfType<GenericTypeError>().Should().HaveCount(1);
        }
    }
}
