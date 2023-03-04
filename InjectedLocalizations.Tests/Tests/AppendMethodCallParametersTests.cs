using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Models;

namespace InjectedLocalizations.Tests
{
    public class AppendMethodCallParametersTests
    {
        [Fact]
        public void Can_build_IValidLocalizations_The_user_0_with_code_1_is_forbiden_method_call_parameters()
        {
            Type type;
            string methodName;
            string expectedResult;

            type = typeof(IValidLocalizations);
            methodName = nameof(IValidLocalizations.The_user_0_with_code_1_is_forbiden);
            expectedResult = "userName, userId";

            MethodInfo method;
            FakeCodeBuilder builder;
            method = type.GetMethod(methodName);
            builder = new FakeCodeBuilder();
            builder.AppendMethodCallParameters(method);

            builder.Buffer.ToString().Should().Be(expectedResult);
        }

        [Fact]
        public void Can_build_IValidLocalizations_That_tree_has_0_tasty_apples_method_call_parameters()
        {
            Type type;
            string methodName;
            string expectedResult;

            type = typeof(IValidLocalizations);
            methodName = nameof(IValidLocalizations.That_tree_has_0_tasty_apples);
            expectedResult = "applesCount";

            MethodInfo method;
            FakeCodeBuilder builder;
            method = type.GetMethod(methodName);
            builder = new FakeCodeBuilder();
            builder.AppendMethodCallParameters(method);

            builder.Buffer.ToString().Should().Be(expectedResult);
        }

        [Fact]
        public void Can_build_IMethodNotMatchingParametersLocalizations_This_Method_has_0_parameter_but_it_has_three_arguments_method_call_parameters()
        {
            Type type;
            string methodName;
            string expectedResult;

            type = typeof(IMethodNotMatchingParametersLocalizations);
            methodName = nameof(IMethodNotMatchingParametersLocalizations.This_Method_has_0_parameter_but_it_has_three_arguments);
            expectedResult = "x, y, z";

            MethodInfo method;
            FakeCodeBuilder builder;
            method = type.GetMethod(methodName);
            builder = new FakeCodeBuilder();
            builder.AppendMethodCallParameters(method);

            builder.Buffer.ToString().Should().Be(expectedResult);
        }
    }
}
