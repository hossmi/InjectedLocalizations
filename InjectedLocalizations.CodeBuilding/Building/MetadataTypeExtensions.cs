using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Building.ValidationErrors;
using InjectedLocalizations.Exceptions;
using JimenaTools.Extensions.Enumerables;
using JimenaTools.Extensions.Types;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Building
{
    public static class MetadataTypeExtensions
    {
        public static string RemoveIPrefix(this Type type)
        {
            string className;

            className = type.Name;

            if (className.StartsWith('I')
                && 2 <= className.Length
                && char.IsLetter(className[1])
                && char.IsUpper(className[1]))
                className = className.Substring(1);

            return className;
        }

        public static bool IsILocalizationsCultureProperty(this PropertyInfo property)
        {
            Type declaringType;

            if (property.Name != nameof(ILocalizations.Culture))
                return false;

            if (property.PropertyType != typeof(CultureInfo))
                return false;

            declaringType = property.DeclaringType;

            if (declaringType.IsInterface)
                return declaringType == Usage.LocalizationsInterfaceType;

            return declaringType.Implements<ILocalizations>();
        }

        public static bool IsILocalizationsCultureProperty(this MemberInfo member)
        {
            return member is PropertyInfo property
                && property.IsILocalizationsCultureProperty();
        }

        public static IEnumerable<MemberInfo> GetMethodsAndPropertiesWithoutObjectMembers(this Type type)
        {
            return type.GetMethodsAndProperties()
                .Where(member => member.DeclaringType != typeof(object));
        }

        public static IEnumerable<MemberInfo> GetMethodsAndProperties(this Type type)
        {
            IEnumerable<PropertyInfo> properties;
            IEnumerable<MethodInfo> propertyAccessors;
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            IEnumerable<Type> types;

            types = type.GetInterfaces().Concat(new[] { type });
            properties = types.SelectMany(t => t.GetProperties(bindingFlags));
            propertyAccessors = properties.SelectMany(p => p.GetAccessors());

            return types
                .SelectMany(t => t.GetMethods(bindingFlags))
                .Substract(propertyAccessors)
                .Cast<MemberInfo>()
                .Concat(properties)
                .OrderBy(m => m.Name);
        }

        public static ParameterInfo[] GetParametersOrEmpty(this MemberInfo member)
        {
            if (member is PropertyInfo)
                return new ParameterInfo[0];

            else if (member is MethodInfo method)
                return method.GetParameters();

            else throw new InvalidInterfacesLocalizationException(new[] { new NotValidMemberError(member) });
        }

        public static string CodeName(this Type type)
        {
            Type[] parameters;
            string parametersCollection;
            IReadOnlyCollection<string> composedParameterNames;
            string name;
            int index;

            type.ShouldBeNotNull(nameof(type));

            if (type.IsGenericType)
            {
                parameters = type.GenericTypeArguments;
                composedParameterNames = parameters
                    .Select(CodeName)
                    .ToArray();

                parametersCollection = string.Join(',', composedParameterNames);
                parametersCollection = $"<{parametersCollection}>";
            }
            else
                parametersCollection = "";

            name = type.Name;
            index = name.IndexOf("`");

            if (0 <= index)
                name = name.Substring(0, index);

            return $"{type.Namespace}.{name}{parametersCollection}";
        }
    }
}
