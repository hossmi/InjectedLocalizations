using System.Text;
using InjectedLocalizations.Building;

namespace InjectedLocalizations.Models
{
    public class FakeCodeBuilder : ICodeBuilder
    {
        private readonly StringBuilder buffer;
        private readonly ICollection<IImplementationType> implementations;
        private readonly ICollection<string> references;
        private readonly Action postAppend;

        public FakeCodeBuilder(string appendSeparator = null)
        {
            this.buffer = new StringBuilder();
            this.implementations = new List<IImplementationType>();
            this.references = new List<string>();

            if (appendSeparator != null)
                this.postAppend = () => this.buffer.Append(appendSeparator);
            else
                this.postAppend = () => { };
        }

        public string Buffer => this.buffer.ToString();

        public IEnumerable<IImplementationType> Implementations => this.implementations;

        public IEnumerable<string> References => this.references;

        public ICodeBuilder Append(string source)
        {
            this.buffer.Append(source);
            this.postAppend();
            return this;
        }

        public ICodeBuilder Deindent(IIndentation token) => this;

        public ICodeBuilder Indent(out IIndentation token)
        {
            token = null;
            return this;
        }

        public void SetImplementation(IImplementationType implementationType) => this.implementations.Add(implementationType);

        public void SetReference(string reference) => this.references.Add(reference);

        public class PrvIndentation : IIndentation { }
    }
}
