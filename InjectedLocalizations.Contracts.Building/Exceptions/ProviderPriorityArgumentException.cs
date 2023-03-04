using System;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class ProviderPriorityArgumentException : ArgumentException
    {
        public ProviderPriorityArgumentException(int priority, Type type)
            : base($"Exists an other localization provider with the same priority.", nameof(priority))
        {
            this.Priority = priority;
            this.Type = type;
        }

        public int Priority { get; }
        public Type Type { get; }
    }
}