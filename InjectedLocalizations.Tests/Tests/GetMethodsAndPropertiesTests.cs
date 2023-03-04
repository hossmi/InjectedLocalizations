using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Models;

namespace InjectedLocalizations.Tests
{
    public class GetMethodsAndPropertiesTests
    {
        [Fact]
        public void Can_get_properties_and_methods_from_IValidLocalizations()
        {
            IReadOnlyCollection<MemberInfo> members = typeof(IValidLocalizations)
                .GetMethodsAndProperties()
                .ToList();

            members.Should().HaveCount(4);
        }

        [Fact]
        public void Can_get_properties_and_methods_from_IEmptyLocalizations()
        {
            IReadOnlyCollection<MemberInfo> members = typeof(IEmptyLocalizations)
                .GetMethodsAndProperties()
                .ToList();

            members.Should().HaveCount(1);
            members.Single().Name.Should().Be(nameof(ILocalizations.Culture));
        }

        [Fact]
        public void Can_get_properties_and_methods_from_INotValidIndexedLocalizations()
        {
            IReadOnlyCollection<MemberInfo> members = typeof(INotValidIndexedLocalizations)
                .GetMethodsAndProperties()
                .ToList();

            members.Should().HaveCount(2);
        }

        [Fact]
        public void Can_get_properties_and_methods_from_IPositionalArgumentsDerivedLocalizations()
        {
            IReadOnlyCollection<MemberInfo> members = typeof(IPositionalArgumentsDerivedLocalizations)
                .GetMethodsAndProperties()
                .ToList();

            members.Should().HaveCount(5);
        }

        [Fact]
        public void Can_get_properties_and_methods_from_B_without_System_Object_members()
        {
            IReadOnlyCollection<MemberInfo> members = typeof(B)
                .GetMethodsAndPropertiesWithoutObjectMembers()
                .ToList();

            members.Should().HaveCount(2);
        }

        [Fact]
        public void Can_get_properties_and_methods_from_B()
        {
            IReadOnlyCollection<MemberInfo> members = typeof(B)
                .GetMethodsAndProperties()
                .ToList();

            members.Should().HaveCount(6);
        }

        [Fact]
        public void Can_remove_I_prefix_from_class_name()
        {
            string className = typeof(ILocalizations).RemoveIPrefix();

            className.Should().Be("Localizations");
        }

        [Fact]
        public void Trying_to_remove_non_exixtent_I_prefix_results_in_same_class_name()
        {
            string className = typeof(object).RemoveIPrefix();

            className.Should().Be("Object");
        }

        [Fact]
        public void Cannot_remove_I_from_I3_interface()
        {
            string className = typeof(I3).RemoveIPrefix();

            className.Should().Be("I3");
        }

        [Fact]
        public void Cannot_remove_I_from_Invisible_interface()
        {
            string className = typeof(Invisible).RemoveIPrefix();

            className.Should().Be("Invisible");
        }

        [Fact]
        public void Can_get_composed_name_of_object()
        {
            string type = typeof(object).CodeName();

            type.Should().Be(typeof(object).FullName);
        }

        [Fact]
        public void Can_get_composed_name_of_list()
        {
            string type = typeof(IList<int>).CodeName();
            type.Should().Be("System.Collections.Generic.IList<System.Int32>");
        }

        [Fact]
        public void Can_get_composed_name_of_dictionary()
        {
            string type = typeof(Dictionary<int, IList<string>>).CodeName();
            type.Should().Be("System.Collections.Generic.Dictionary<System.Int32,System.Collections.Generic.IList<System.String>>");
        }

        [Fact]
        public void Can_get_composed_name_of_complex_type()
        {
            string type = typeof(Dictionary<Tuple<int, string, ICollection<decimal>>, IDictionary<double, string>>).CodeName();
            type.Should().Be("System.Collections.Generic.Dictionary<System.Tuple<System.Int32,System.String,System.Collections.Generic.ICollection<System.Decimal>>,System.Collections.Generic.IDictionary<System.Double,System.String>>");
        }
    }
}
