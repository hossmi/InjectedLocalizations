using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using InjectedLocalizations.Building;
using Microsoft.CodeAnalysis;

namespace InjectedLocalizations.Exceptions
{
    [Serializable]
    public class CompilationLocalizationException : LocalizationException
    {
        public CompilationLocalizationException(ICodeRequest request, IReadOnlyCollection<Diagnostic> diagnostics)
            : base("There have been errors generating localizations assembly.")
        {
            this.Request = request;
            this.Diagnostics = diagnostics;
        }

        protected CompilationLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ICodeRequest Request { get; }
        public IReadOnlyCollection<Diagnostic> Diagnostics { get; }
    }
}