using System;

namespace InjectedLocalizations.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public abstract class AbstractSentenceAttribute : Attribute { }
}
