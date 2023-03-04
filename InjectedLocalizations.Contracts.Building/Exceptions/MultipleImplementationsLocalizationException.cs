using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class MultipleImplementationsLocalizationException : LocalizationException
    {
        public MultipleImplementationsLocalizationException(Assembly assembly, Type interfaceType, IReadOnlyCollection<Type> implementationTypes)
            : base($"Multiple implementations were found at the new assembly.")
        {
            this.Assembly = assembly;
            this.InterfaceType = interfaceType;
            this.ImplementationTypes = implementationTypes;
        }

        protected MultipleImplementationsLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Assembly Assembly { get; }
        public Type InterfaceType { get; }
        public IReadOnlyCollection<Type> ImplementationTypes { get; }
    }
}