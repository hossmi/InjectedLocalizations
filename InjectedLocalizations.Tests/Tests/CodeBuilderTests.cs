using System.Globalization;
using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Models;
using NSubstitute;

namespace InjectedLocalizations.Tests
{
    public class CodeBuilderTests
    {
        [Fact]
        public void Inittial_codebuilder_has_no_implementations()
        {
            ICodeBuilder builder = new CodeBuilder();

            builder.Implementations.Should().BeEmpty();
        }

        [Fact]
        public void Cannot_set_two_implementations_for_same_couple_of_culture_and_type()
        {
            ICodeBuilder builder = new CodeBuilder();

            builder.SetImplementation(new FakeImplementationType
            {
                Culture = new CultureInfo("en-US"),
                ImplementationTypeName = "Test",
                InterfaceType = typeof(IEmptyLocalizations),
            });

            builder.SetImplementation(new FakeImplementationType
            {
                Culture = new CultureInfo("en-US"),
                ImplementationTypeName = "Test2",
                InterfaceType = typeof(IEmptyLocalizations),
            });

            builder.Implementations.Should().HaveCount(1);
            builder
                .Implementations
                .Single()
                .ImplementationTypeName.Should().Be("Test2");
        }

        [Fact]
        public void Can_Append()
        {
            CodeBuilder builder = new CodeBuilder();

            builder.Append("test");

            builder.SourceCode.Should().Be(@"test
");
        }

        [Fact]
        public void Can_Indent()
        {
            CodeBuilder builder = new CodeBuilder();

            builder.Indent(out IIndentation indentation);
            builder.Indent(out IIndentation indentation2);
            builder.Deindent(indentation2);
            builder.Deindent(indentation);
        }

        [Fact]
        public void Cannot_Deindent_unpaired_indentation()
        {
            CodeBuilder builder = new CodeBuilder();
            Action deindent;

            builder.Indent(out IIndentation indentation);
            deindent = () => builder.Deindent(indentation);

            deindent();
            deindent.Should().ThrowExactly<DeindentationUnderflowLocalizationException>();
        }

        [Fact]
        public void Cannot_Deindent_unmatched_indentation()
        {
            CodeBuilder builder = new CodeBuilder();
            Action deindent;

            builder.Indent(out IIndentation indentation1);
            builder.Indent(out IIndentation indentation2);
            deindent = () => builder.Deindent(indentation1);

            deindent.Should().ThrowExactly<WrongDeindentationLocalizationException>();
        }

        [Fact]
        public void Cannot_Deindent_foreign_indentation()
        {
            CodeBuilder builder = new CodeBuilder();
            Action deindent;
            IIndentation fakeIndentation;

            builder.Indent(out IIndentation indentation);
            fakeIndentation = Substitute.For<IIndentation>();
            deindent = () => builder.Deindent(fakeIndentation);

            deindent.Should().ThrowExactly<WrongDeindentationLocalizationException>();
        }

        [Fact]
        public void Can_Indent_and_add_source()
        {
            CodeBuilder builder = new CodeBuilder();

            builder.Indent(out IIndentation indentation);
            builder.Append("test");
            builder.Deindent(indentation);

            builder.SourceCode.Should().Be($@"{'\t'}test
");
        }

        [Fact]
        public void Can_set_explicit_reference()
        {
            CodeBuilder builder = new CodeBuilder();

            string location = typeof(CodeBuilderTests).GetTypeInfo().Assembly.Location;

            builder.SetReference(location);

            builder.References.Should().HaveCount(1);
            builder
                .References
                .Single()
                .Should().Be(location);
        }

        [Fact]
        public void Can_set_same_reference_only_once()
        {
            CodeBuilder builder = new CodeBuilder();

            string location1 = typeof(CodeBuilderTests)
                .GetTypeInfo()
                .Assembly
                .Location;

            string location2 = typeof(CodeBuilderTests)
                .GetTypeInfo()
                .Assembly
                .Location;

            builder.SetReference(location1);
            builder.SetReference(location2);

            builder.References.Should().HaveCount(1);
            builder
                .References
                .Single()
                .Should().Be(location1);
        }

        [Fact]
        public void Can_set_framework_reference()
        {
            CodeBuilder builder = new CodeBuilder();

            string location = "netstandard.dll".AsFullPathReference();

            builder.SetReference("netstandard.dll");

            builder.References.Should().HaveCount(1);
            builder
                .References
                .Single()
                .Should().Be(location);
        }

        [Fact]
        public void Can_get_framework_reference()
        {
            string reference = "netstandard.dll";
            string location = reference.AsFullPathReference();

            location.Should().Contain(reference);
        }

        [Fact]
        public void Inittial_codebuilder_has_no_references()
        {
            CodeBuilder builder = new CodeBuilder();

            builder.References.Should().BeEmpty();
        }

        [Fact]
        public void Inittial_source_code_should_be_empty()
        {
            CodeBuilder builder = new CodeBuilder();

            builder.SourceCode.Should().BeEmpty();
        }
    }
}
