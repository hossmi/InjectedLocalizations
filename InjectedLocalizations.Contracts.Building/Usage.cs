using System;
using System.Collections.Generic;
using System.Linq;

namespace InjectedLocalizations
{
    public static class Usage
    {
        static Usage()
        {
            string types;
            IEnumerable<string> typeNames;

            ValidReturnTypes = new[] { typeof(string) };
            LocalizationsInterfaceType = typeof(ILocalizations);

            typeNames = ValidReturnTypes.Select(t => t.FullName);
            types = string.Join(", ", typeNames);

            Message = $@"In order to run Localization system, interface types must follow the next structure:
In relation to interfaces:
    - Interfaces must inherit from {LocalizationsInterfaceType.FullName}.
    - You can nest interfeces as long as all of them inherit from {LocalizationsInterfaceType.Name}.

With respect to interface methods and properties:
    - Methods must have at least one parameter (of any type).
    - Methods parameters cannot have in, ref or out modiffiers.
    - Methods name car reference its parameters with positional indexes on its own name. An out of range index cannot be translated as the name of a parameter, in consequence, it will be left as a raw number.
    - Any number on a property name will be left as a raw number.
    - Properties must be read only.
    - Methods and Properties must return one of these types: {types}.
";
        }

        public static string Message { get; }
        public static IReadOnlyCollection<Type> ValidReturnTypes { get; }
        public static Type LocalizationsInterfaceType { get; }
    }
}
