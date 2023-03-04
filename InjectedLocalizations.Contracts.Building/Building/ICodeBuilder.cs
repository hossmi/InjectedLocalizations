using System.Collections.Generic;

namespace InjectedLocalizations.Building
{
    public interface ICodeBuilder
    {
        IEnumerable<IImplementationType> Implementations { get; }
        void SetImplementation(IImplementationType implementationType);
        ICodeBuilder Append(string source);
        void SetReference(string reference);
        ICodeBuilder Indent(out IIndentation token);
        ICodeBuilder Deindent(IIndentation token);
    }
}