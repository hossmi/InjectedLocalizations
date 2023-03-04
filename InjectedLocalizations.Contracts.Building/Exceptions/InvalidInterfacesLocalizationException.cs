using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using InjectedLocalizations.Building;

namespace InjectedLocalizations.Exceptions
{
    public class InvalidInterfacesLocalizationException : LocalizationException
    {
        public InvalidInterfacesLocalizationException(IEnumerable<IError> errors) : base(Usage.Message)
        {
            this.ValidationErrors = errors
                .OrderBy(e => e.Type.Name)
                .ThenBy(e => e.Message)
                .ToArray();
        }

        protected InvalidInterfacesLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IError[] ValidationErrors { get; }
    }
}