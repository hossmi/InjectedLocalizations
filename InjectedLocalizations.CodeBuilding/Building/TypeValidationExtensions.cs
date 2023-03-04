using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Building.ValidationErrors;
using JimenaTools.Extensions.Enumerables;
using JimenaTools.Extensions.Types;
using JimenaTools.Extensions.Validations;
using Microsoft.CodeAnalysis;

namespace InjectedLocalizations.Building
{
    public static class TypeValidationExtensions
    {
        public static IEnumerable<Type> FilterCandidateInterfaces(this IEnumerable<Type> interfaceTypes)
        {
            return interfaceTypes
                .Where(t => t.IsInterface)
                .Where(t => t != Usage.LocalizationsInterfaceType)
                .Where(TypeInheritanceExtensions.InheritsFrom<ILocalizations>);
        }

        public static IEnumerable<IError> ValidateLocalizationInterface(this Type interfaceType)
        {
            IEnumerable<MemberInfo> members;
            bool hasTypeErrors;

            interfaceType.ShouldBeNotNull(nameof(interfaceType));

            if (!interfaceType.IsInterface)
            {
                yield return new NotInterfaceTypeError(interfaceType);
                yield break;
            }

            hasTypeErrors = false;

            if (!interfaceType.InheritsFrom<ILocalizations>())
            {
                yield return new ILocalizationsNotDerivedTypeError(interfaceType);
                hasTypeErrors = true;
            }

            if (interfaceType.IsGenericType)
            {
                yield return new GenericTypeError(interfaceType);
                hasTypeErrors = true;
            }

            if (hasTypeErrors)
                yield break;

            members = interfaceType
                .GetMethodsAndProperties()
                .Where(m => m.DeclaringType != Usage.LocalizationsInterfaceType);

            foreach (MemberInfo member in members)
            {
                if (member is PropertyInfo property)
                {
                    if (!property.CanRead)
                        yield return new NonReadablePropertyError(property);

                    if (property.CanWrite)
                        yield return new WritablePropertyError(property);

                    if (property.GetIndexParameters().Any())
                        yield return new IndexedPropertyError(property);

                    if (!property.PropertyType.IsAnyOf(Usage.ValidReturnTypes))
                        yield return new ReturnTypeMemberError(property);

                    if (property.IsSpecialName)
                        yield return new SpecialNameMemberError(property);

                    continue;
                }

                if (member is MethodInfo method)
                {
                    if (!method.ReturnType.IsAnyOf(Usage.ValidReturnTypes))
                        yield return new ReturnTypeMemberError(method);

                    if (method.ContainsGenericParameters)
                        yield return new GenericParametersMethodError(method);

                    if (method.IsSpecialName)
                        yield return new SpecialNameMemberError(method);

                    continue;
                }

                yield return new NotValidMemberError(member);
            }
        }
    }
}
