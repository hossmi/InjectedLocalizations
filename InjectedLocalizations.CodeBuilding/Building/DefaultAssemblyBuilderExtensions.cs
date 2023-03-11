using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Abstractions;
using InjectedLocalizations.Building.ValidationErrors;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Providers;
using JimenaTools.Extensions.Enumerables;
using JimenaTools.Extensions.Validations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace InjectedLocalizations.Building
{
    public static class DefaultAssemblyBuilderExtensions
    {
        public static ICodeBuilder BuildSource(this ICodeBuilder builder
            , Type interfaceType
            , ILocalizationsProviderConfiguration providersConfiguration
            , ILocalizationsCultureConfiguration cultureConfiguration)
        {
            IReadOnlyCollection<ILocalizationRequest> typeRequests;

            typeRequests = cultureConfiguration
                .Cultures
                .Select(culture => new PrvLocalizationTypeRequest
                {
                    InterfaceType = interfaceType,
                    Culture = culture,
                })
                .ToList();

            if (typeRequests.Count == 0)
                throw new MissingCulturesArgumentException($"{nameof(cultureConfiguration)}.{nameof(cultureConfiguration.Cultures)}");

            builder
                .Append($"namespace {nameof(InjectedLocalizations)}.Generated")
                .OpenBlock(out IIndentation namespaceIndentation)
                    .BuildSpecificCultureLocalizationClasses(typeRequests, providersConfiguration.Providers)
                    .BuildMotherClasses(cultureConfiguration.Default, () => Guid.NewGuid().ToString("N"))
                .CloseBlock(namespaceIndentation);

            builder.SetReference("netstandard.dll");
            builder.SetReference("System.Runtime.dll");
            builder.SetReference<ILocalSymbol>();
            //builder.SetReference<IRequestCultureFeature>(); // TODO estoooo ya no
            builder.SetReference(Usage.LocalizationsInterfaceType);
            builder.SetReference<AbstractLocalizations>();
            builder.SetReference<IServiceProvider>();

            return builder;
        }

        public static ICodeBuilder BuildSpecificCultureLocalizationClasses(this ICodeBuilder builder
            , IEnumerable<ILocalizationRequest> requests
            , IEnumerable<ILocalizationsProvider> providers)
        {
            foreach (ILocalizationRequest request in requests)
            {
                PrvObservableCodeBuilder observableBuilder;

                observableBuilder = new PrvObservableCodeBuilder(builder);

                foreach (ILocalizationsProvider provider in providers)
                {
                    provider.TryBuildLocalizationFor(request, observableBuilder);

                    if (observableBuilder.Used)
                        break;
                }

                if (!observableBuilder.Used)
                    throw new SpecificTypeNotGeneratedLocalizationException(request, providers);
            }

            return builder;
        }

        public static ICodeBuilder BuildMotherClasses(this ICodeBuilder builder, CultureInfo defaultCulture, Func<string> getRandomChunk)
        {
            builder
                .Implementations
                .OrderBy(i => i.InterfaceType.FullName)
                .ThenBy(i => i.Culture.Name)
                .ThenBy(i => i.ImplementationTypeName)
                .GroupBy(g => g.InterfaceType)
                .AndApply(g => builder.BuildMotherClass(g.Key, g, defaultCulture, getRandomChunk()));

            return builder;
        }

        public static ICodeBuilder BuildMotherClass(this ICodeBuilder builder
            , Type interfaceType
            , IEnumerable<IImplementation> implementationTypes
            , CultureInfo defaultCulture
            , string randomChunk)
        {
            string className, typeDescriptorsVarName, typeDescriptorsVarType, typeDescriptorImplementationType, cultureInfoFullName, defaultCultureVarName;
            IEnumerator<IImplementation> implementationsEnumerator;

            implementationsEnumerator = implementationTypes.GetEnumerator();

            if (!implementationsEnumerator.MoveNext())
                throw new EmptyMotherClassLocalizationException(builder, interfaceType);

            className = $"Mother_{randomChunk}_{interfaceType.RemoveIPrefix()}";
            typeDescriptorsVarName = "typeDescriptors";
            typeDescriptorsVarType = typeof(IDictionary<CultureInfo, Type>).CodeName();
            typeDescriptorImplementationType = typeof(Dictionary<CultureInfo, Type>).CodeName();

            cultureInfoFullName = typeof(CultureInfo).FullName;
            defaultCultureVarName = "defaultCulture";

            builder
                .Append($"public class {className} : {typeof(AbstractLocalizations).FullName}<{interfaceType.FullName}>, {interfaceType.FullName}")
                .OpenBlock(out IIndentation classIndentation)
                    .AppendLine()
                    .Append($"private static readonly {typeDescriptorsVarType} {typeDescriptorsVarName};")
                    .Append($"private static readonly {cultureInfoFullName} {defaultCultureVarName};")
                    .AppendLine()
                    .Append($"static {className}()")
                    .OpenBlock(out IIndentation staticCtorIndent)
                        .Append($"{typeDescriptorsVarName} = new {typeDescriptorImplementationType}();")
                        .AppendDictionaryInitializations(implementationsEnumerator, typeDescriptorsVarName, cultureInfoFullName)
                        .AppendLine()
                        .Append($@"{defaultCultureVarName} = new {cultureInfoFullName}(""{defaultCulture.Name}"");")
                    .CloseBlock(staticCtorIndent)
                    .AppendLine()
                    .Append($@"public {className}({typeof(IServiceProvider).FullName} serviceProvider) : base(serviceProvider, {typeDescriptorsVarName}) {{ }}")
                    .AppendLine()
                    .AppendMotherMembers(interfaceType)
                .CloseBlock(classIndentation);

            builder.SetReference(interfaceType);
            builder.SetReference(typeof(Dictionary<,>));
            builder.SetReference(typeof(IDictionary<,>));
            builder.SetReference<IDictionary<CultureInfo, Type>>();

            return builder;
        }

        public static void SetReference<T>(this ICodeBuilder builder)
        {
            builder.SetReference(typeof(T));
        }

        public static void SetReference(this ICodeBuilder builder, Type type)
        {
            string location = type
                .ShouldBeNotNull(nameof(type))
                .GetTypeInfo()
                .Assembly
                .Location;

            builder.SetReference(location);
        }

        public static ICodeBuilder AppendDictionaryInitializations(this ICodeBuilder builder
            , IEnumerator<IImplementation> ImplementationsEnumerator, string typeDescriptorsVar, string cultureFullName)
        {
            do
            {
                IImplementation implementationType;

                implementationType = ImplementationsEnumerator.Current
                    ?? throw new ArgumentNullException("Enumerator should have at least one item.");

                builder.AppendDictionaryInitialization(implementationType, typeDescriptorsVar, cultureFullName);
            } while (ImplementationsEnumerator.MoveNext());

            return builder;
        }

        public static void AppendDictionaryInitialization(this ICodeBuilder builder
            , IImplementation implementationType, string typeDescriptorsVar, string cultureFullName)
        {
            builder
                .Append($"{typeDescriptorsVar}.Add(")
                    .Indent(out IIndentation indentation)
                    .Append($@"new {cultureFullName}(""{implementationType.Culture.Name}""),")
                    .Append($"typeof({implementationType.ImplementationTypeName})")
                    .Deindent(indentation)
                .Append($");");
        }

        public static ICodeBuilder AppendMotherMembers(this ICodeBuilder builder, Type interfaceType)
        {
            IEnumerable<MemberInfo> members;

            members = interfaceType.GetMethodsAndProperties();

            foreach (MemberInfo member in members)
            {
                if (member is PropertyInfo property)
                    builder.AppendMotherProperty(property);

                else if (member is MethodInfo method)
                    builder.AppendMotherMethod(method);

                else throw new InvalidInterfacesLocalizationException(new[]
                {
                    new NotValidMemberError(member),
                });
            }

            return builder;
        }

        public static ICodeBuilder AppendMotherProperty(this ICodeBuilder builder, PropertyInfo property)
        {
            return builder.Append($"public {property.PropertyType.FullName} {property.Name} => base.instance.{property.Name};");
        }

        public static ICodeBuilder AppendMotherMethod(this ICodeBuilder builder, MethodInfo method)
        {
            return builder
                .Append($"public {method.ReturnType.FullName} {method.Name}")
                .Append("(").AppendMethodSignatureParameters(method).Append(")")
                .OpenBlock(out IIndentation indentation)
                    .Append($"return base.instance.{method.Name}").Append("(").AppendMethodCallParameters(method).Append(");")
                .CloseBlock(indentation);
        }

        public static Assembly CompileAssembly(this ICodeRequest request)
        {
            Assembly assemblyResult;
            SyntaxTree sourceTree;
            string assemblyName;
            IEnumerable<PortableExecutableReference> references;
            CSharpCompilation compilation;

            sourceTree = SyntaxFactory.ParseSyntaxTree(request.SourceCode);
            assemblyName = $"Generated.{Guid.NewGuid().ToString("N")}.dll";

            references = request
                .References
                .Select(reference => MetadataReference.CreateFromFile(reference));

            compilation = CSharpCompilation
                .Create(assemblyName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(sourceTree);

            using (MemoryStream stream = new MemoryStream(1024 * 1024)) // TODO receive the stream by parameter
            {
                EmitResult compilationResult;

                compilationResult = compilation.Emit(stream);

                if (compilationResult.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    assemblyResult = Assembly.Load(stream.ToArray());
                }
                else
                {
                    throw new CompilationLocalizationException(request, compilationResult.Diagnostics);
                }
            }

            return assemblyResult;
        }

        private class PrvLocalizationTypeRequest : ILocalizationRequest
        {
            public Type InterfaceType { get; internal set; }
            public CultureInfo Culture { get; internal set; }
        }

        private class PrvObservableCodeBuilder : ICodeBuilder
        {
            private readonly ICodeBuilder builder;
            private readonly List<string> addedReferences;
            private readonly List<string> addedSources;
            private readonly List<IImplementationType> addedImplementations;

            public IEnumerable<IImplementationType> Implementations => builder.Implementations;

            public PrvObservableCodeBuilder(ICodeBuilder builder)
            {
                this.builder = builder ?? throw new ArgumentNullException(nameof(builder));
                addedReferences = new List<string>();
                addedSources = new List<string>();
                addedImplementations = new List<IImplementationType>();
            }

            public bool Used => addedReferences.Any() || addedSources.Any() || addedImplementations.Any();
            public IReadOnlyCollection<string> AddedReferences => addedReferences;
            public IReadOnlyCollection<string> AddedSources => addedSources;
            public IReadOnlyCollection<IImplementationType> AddedImplementations => addedImplementations;

            public void SetImplementation(IImplementationType implementationType)
            {
                builder.SetImplementation(implementationType);
                addedImplementations.Add(implementationType);
            }

            public ICodeBuilder Append(string source)
            {
                builder.Append(source);
                addedSources.Add(source);
                return this;
            }

            public ICodeBuilder Deindent(IIndentation token)
            {
                builder.Deindent(token);
                return this;
            }

            public ICodeBuilder Indent(out IIndentation token)
            {
                builder.Indent(out token);
                return this;
            }

            public void SetReference(string reference)
            {
                builder.SetReference(reference);
                addedReferences.Add(reference);
            }
        }
    }
}
