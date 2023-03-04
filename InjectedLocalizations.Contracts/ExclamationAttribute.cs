using System;
using InjectedLocalizations.Attributes;

namespace InjectedLocalizations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ExclamationAttribute : AbstractSentenceAttribute { }
}
