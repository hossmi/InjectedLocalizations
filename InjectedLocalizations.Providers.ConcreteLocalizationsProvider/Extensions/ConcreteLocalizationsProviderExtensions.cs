using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using JimenaTools.Extensions.Types;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Extensions
{
    public static class ConcreteLocalizationsProviderExtensions
    {
        private static readonly Type[] constructorParameters;

        static ConcreteLocalizationsProviderExtensions()
        {
            constructorParameters = new Type[0];
        }

        public static IDictionary<Type, IDictionary<CultureInfo, Type>> BuildMasterDictionary(this ILocalizationsAssembliesConfiguration configuration)
        {
            return configuration
                .ShouldBeNotNull(nameof(configuration))
                .Assemblies
                .SelectMany(ass => ass.ExportedTypes)
                .Where(t => t.IsClass)
                .Where(TypeInheritanceExtensions.Implements<ILocalizations>)
                .SelectMany(t => t
                    .GetInterfaces()
                    .Where(i => i != Usage.LocalizationsInterfaceType)
                    .Select(i => new InterfaceCultureImplementationRow
                    {
                        Interface = i,
                        Implementation = t,
                        Culture = t.GetLocalizationsCulture(),
                    }))
                .AsMasterDictionary();
        }

        public static CultureInfo GetLocalizationsCulture(this Type type)
        {
            ConstructorInfo constructor;
            ILocalizations instance;

            constructor = type
                .GetConstructor(constructorParameters)
                ?? throw new ConstructorNotFoundLocalizationException(type, constructorParameters);

            instance = (ILocalizations)constructor.Invoke(new object[0]);

            return instance.Culture;
        }

        public static IDictionary<Type, IDictionary<CultureInfo, Type>> AsMasterDictionary(this IEnumerable<InterfaceCultureImplementationRow> rows)
        {
            ICollection<string> errors;
            Dictionary<Type, IDictionary<CultureInfo, Type>> implementations;

            errors = new List<string>();
            implementations = new Dictionary<Type, IDictionary<CultureInfo, Type>>();

            foreach (InterfaceCultureImplementationRow row in rows)
            {
                if (!implementations.TryGetValue(row.Interface, out IDictionary<CultureInfo, Type> cultures))
                {
                    cultures = new Dictionary<CultureInfo, Type>();
                    implementations.Add(row.Interface, cultures);
                }

                if (cultures.TryGetValue(row.Culture, out Type implementation))
                    errors.Add($@"There is already an implementation for <{row.Interface.FullName}, {row.Culture}>.
Existing: {implementation.FullName}
Tried to add: {row.Implementation.FullName}
");
                else
                    cultures.Add(row.Culture, row.Implementation);
            }

            if (errors.Any())
                throw new TooManyImplementationsLocalizationException(errors);

            return implementations;
        }
    }
}
