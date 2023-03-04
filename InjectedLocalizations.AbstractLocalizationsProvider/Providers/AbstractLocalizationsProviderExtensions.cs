using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Building;
using InjectedLocalizations.Building.ValidationErrors;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Providers;
using JimenaTools.Extensions.Enumerables;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Providers
{
    public static class AbstractLocalizationsProviderExtensions
    {
        public static ICodeBuilder BuildSpecificCultureLocalizationClass(this ICodeBuilder builder
            , ILocalizationResponse response
            , string randomChunk)
        {
            string className;

            className = response.Request.BuildSpecificCultureClassName(randomChunk);

            builder
                .Append($"internal class {className} : {response.Request.InterfaceType.FullName}")
                .OpenBlock(out IIndentation classIndentation)
                    .AppendSpecificClassMembers(response)
                .CloseBlock(classIndentation);

            builder.SetImplementation(new PrvImplementationType
            {
                Culture = response.Request.Culture,
                InterfaceType = response.Request.InterfaceType,
                ImplementationTypeName = className,
            });

            return builder;
        }

        public static string BuildSpecificCultureClassName(this ILocalizationRequest request, string randomChunk)
        {
            string className, cultureName, interfaceName;

            cultureName = request.Culture.GetEnglishSafeClassName();
            interfaceName = request.InterfaceType.RemoveIPrefix();
            className = $"{cultureName}_{randomChunk}_{interfaceName}";

            return className;
        }

        public static string GetEnglishSafeClassName(this CultureInfo culture)
        {
            return string
                .Join("", culture
                    .EnglishName
                    .Where(char.IsLetterOrDigit));
        }

        public static ICodeBuilder AppendSpecificClassMembers(this ICodeBuilder builder, ILocalizationResponse response)
        {
            IOrderedEnumerable<KeyValuePair<MemberInfo, string>> pairs;

            pairs = response
                .Members
                .OrderBy(m => m.Key.Name);

            foreach (KeyValuePair<MemberInfo, string> pair in pairs)
            {
                if (pair.Key is PropertyInfo property)
                    builder.AppendProperty(property, pair.Value);

                else if (pair.Key is MethodInfo method)
                    builder.AppendMethod(method, pair.Value);

                else throw new InvalidInterfacesLocalizationException(new[] { new NotValidMemberError(pair.Key) });
            }

            response
                .Request
                .InterfaceType
                .GetMethodsAndProperties()
                .OfType<PropertyInfo>()
                .Where(MetadataTypeExtensions.IsILocalizationsCultureProperty)
                .Apply(p => builder.AppendCultureProperty(p, response.Request.Culture))
                .ShouldBeSingle(() => new InvalidInterfacesLocalizationException(new[] { new ILocalizationsNotDerivedTypeError(response.Request.InterfaceType) }));

            return builder;
        }

        public static void AppendProperty(this ICodeBuilder builder, PropertyInfo property, string value)
        {
            builder
                .Append($"public {property.PropertyType.FullName} {property.Name}")
                .OpenBlock(out IIndentation propertyIndentation)
                    .Append("get")
                    .OpenBlock(out IIndentation i)
                        .Append($@"return $@""{value}"";")
                    .CloseBlock(i)
                .CloseBlock(propertyIndentation)
                .AppendLine();
        }

        public static void AppendCultureProperty(this ICodeBuilder builder, PropertyInfo property, CultureInfo culture)
        {
            string cultureInfoType;

            cultureInfoType = property.PropertyType.FullName;

            builder
                .Append($"public {cultureInfoType} {property.Name}")
                .Append($"{{ get; }} = new {cultureInfoType}(\"{culture.Name}\");")
                .AppendLine();
        }

        public static void AppendMethod(this ICodeBuilder builder, MethodInfo method, string value)
        {
            builder
                .Append($"public {method.ReturnType.FullName} {method.Name}")
                .Append("(")
                    .AppendMethodSignatureParameters(method)
                .Append(")")
                .OpenBlock(out IIndentation i)
                    .Append($@"return $@""{value}"";")
                .CloseBlock(i)
                .AppendLine();
        }

        private class PrvImplementationType : IImplementationType
        {
            public CultureInfo Culture { get; set; }
            public Type InterfaceType { get; set; }
            public string ImplementationTypeName { get; set; }
        }
    }
}
