using System.IO;
using System.Reflection;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Building
{
    public static class CodeBuilderExtensions
    {
        public static string AsFullPathReference(this string reference)
        {
            string referencePath;

            reference.ShouldBeFilled(nameof(reference));
            referencePath = Path.GetFullPath(reference);

            if (!File.Exists(referencePath))
            {
                string netPath;

                netPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
                referencePath = Path.Combine(netPath, reference);

                if (!File.Exists(referencePath))
                    throw new FileNotFoundException("Cannot find reference assembly.", referencePath);
            }

            return referencePath;
        }

        public static ICodeBuilder AppendMethodSignatureParameters(this ICodeBuilder builder, MethodInfo method)
        {
            ParameterInfo[] parameters;
            ParameterInfo parameter;

            parameters = method.GetParameters();
            parameter = parameters[0];

            builder.Append($"{parameter.ParameterType.FullName} {parameter.Name}");

            for (int i = 1, n = parameters.Length; i < n; ++i)
            {
                parameter = parameters[i];

                builder
                    .Append($", {parameter.ParameterType.FullName} {parameter.Name}");
            }

            return builder;
        }

        public static ICodeBuilder AppendMethodCallParameters(this ICodeBuilder builder, MethodInfo method)
        {
            ParameterInfo[] parameters;
            ParameterInfo parameter;

            parameters = method.GetParameters();
            parameter = parameters[0];

            builder.Append($"{parameter.Name}");

            for (int i = 1, n = parameters.Length; i < n; ++i)
                builder.Append($", {parameters[i].Name}");

            return builder;
        }

        public static ICodeBuilder AppendLine(this ICodeBuilder builder) => builder.Append("");

        public static ICodeBuilder OpenBlock(this ICodeBuilder builder, out IIndentation indentation) => builder
            .Append("{")
            .Indent(out indentation);

        public static ICodeBuilder CloseBlock(this ICodeBuilder builder, IIndentation indentation) => builder
            .Deindent(indentation)
            .Append("}");

    }
}
