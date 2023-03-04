using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Models;

namespace InjectedLocalizations.Tests
{
    public class AppendMethodSignatureParametersTests
    {
        [Fact]
        public void Can_build_IValidLocalizations_The_user_0_with_code_1_is_forbiden_method_signature_parameters()
        {
            MethodInfo method;
            FakeCodeBuilder builder;

            method = typeof(IValidLocalizations)
                .GetMethod(nameof(IValidLocalizations.The_user_0_with_code_1_is_forbiden));

            builder = new FakeCodeBuilder();
            builder.AppendMethodSignatureParameters(method);

            builder.Buffer.ToString().Should().Be($"{typeof(string).FullName} userName, {typeof(Guid).FullName} userId");
        }

        [Fact]
        public void Can_build_IValidLocalizations_That_tree_has_0_tasty_apples_method_signature_parameters()
        {
            MethodInfo method;
            FakeCodeBuilder builder;

            method = typeof(IValidLocalizations)
                .GetMethod(nameof(IValidLocalizations.That_tree_has_0_tasty_apples));

            builder = new FakeCodeBuilder();
            builder.AppendMethodSignatureParameters(method);

            builder.Buffer.ToString().Should().Be($"{typeof(int).FullName} applesCount");
        }

        [Fact]
        public void Can_build_IMethodNotMatchingParametersLocalizations_This_Method_has_0_parameter_but_it_has_three_arguments_method_signature_parameters()
        {
            MethodInfo method;
            FakeCodeBuilder builder;

            method = typeof(IMethodNotMatchingParametersLocalizations)
                .GetMethod(nameof(IMethodNotMatchingParametersLocalizations.This_Method_has_0_parameter_but_it_has_three_arguments));

            builder = new FakeCodeBuilder();
            builder.AppendMethodSignatureParameters(method);

            builder.Buffer.ToString().Should().Be($"{typeof(int).FullName} x, {typeof(string).FullName} y, {typeof(double).FullName} z");
        }
    }
}
