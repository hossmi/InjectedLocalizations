using System.Reflection;
using FluentAssertions;
using InjectedLocalizations.Building;
using InjectedLocalizations.Exceptions;
using NSubstitute;

namespace InjectedLocalizations.Tests
{
    public class CompilationExtensionsTests
    {
        [Fact]
        public void Can_build_new_assembly()
        {
            ICodeRequest request;
            Assembly assemblyResult;
            Type compiledType;

            request = Substitute.For<ICodeRequest>();

            request.SourceCode.Returns(@"
using System;

namespace Testing
{
    public class SumTest
    {
        public static int Sum(int a, int b)
        {
            return a + b;
        }
    }
}");

            request.References.Returns(new string[]
            {
                "netstandard.dll".AsFullPathReference(),
                typeof(object).Assembly.Location.AsFullPathReference(),
            });

            assemblyResult = request.CompileAssembly();

            assemblyResult.ExportedTypes.Should().ContainSingle();
            compiledType = assemblyResult.ExportedTypes.Single();
            compiledType.Name.Should().Be("SumTest");
        }

        [Fact]
        public void Can_build_new_assembly_from_empty_code()
        {
            ICodeRequest request;
            Assembly assemblyResult;

            request = Substitute.For<ICodeRequest>();
            request.SourceCode.Returns(@"");
            request.References.Returns(new string[]
            {
                "netstandard.dll".AsFullPathReference(),
            });

            assemblyResult = request.CompileAssembly();

            assemblyResult.ExportedTypes.Should().BeEmpty();
        }

        [Fact]
        public void Can_build_new_assembly_from_empty_namespace()
        {
            ICodeRequest request;
            Assembly assemblyResult;

            request = Substitute.For<ICodeRequest>();
            request.SourceCode.Returns(@"namespace Testing { }");
            request.References.Returns(new string[]
            {
                "netstandard.dll".AsFullPathReference(),
            });

            assemblyResult = request.CompileAssembly();

            assemblyResult.ExportedTypes.Should().BeEmpty();
        }

        [Fact]
        public void Cannot_build_new_assembly_from_non_compiling_code()
        {
            ICodeRequest request;
            Assembly assemblyResult;

            request = Substitute.For<ICodeRequest>();
            request.SourceCode.Returns(@"namespace Testing { boooooom! }");
            request.References.Returns(new string[]
            {
                "netstandard.dll".AsFullPathReference(),
            });

            Action build = () => assemblyResult = request.CompileAssembly();

            build.Should().ThrowExactly<CompilationLocalizationException>();
        }
    }
}
