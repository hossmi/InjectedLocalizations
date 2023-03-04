using System.Collections.Generic;

namespace InjectedLocalizations.Building
{
    public interface ICodeRequest
    {
        IEnumerable<string> References { get; }
        string SourceCode { get; }
    }
}