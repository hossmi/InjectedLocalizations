using System;

namespace InjectedLocalizations.Building.ValidationErrors
{
    public abstract class AbstractTypeError : AbstractError
    {
        public AbstractTypeError(string message, Type type) : base(message, type)
        {
        }
    }
}
