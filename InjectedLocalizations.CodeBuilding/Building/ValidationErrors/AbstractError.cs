using System;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public abstract class AbstractError : IError
    {
        protected AbstractError(string message, Type type)
        {
            if (message.ShouldBeFilled(nameof(message)).EndsWith('.'))
                message = message.Substring(0, message.Length - 1);

            this.Message = message;
            this.Type = type;
        }

        public string Message { get; }
        public Type Type { get; }

        public override string ToString() => $"{this.Message}: {this.Type.Name}";
    }
}
