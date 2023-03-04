using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using InjectedLocalizations.Exceptions;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Building
{
    public class CodeBuilder : ICodeBuilder, ICodeRequest
    {
        private readonly StringBuilder buffer;
        private readonly ICollection<string> references;
        private readonly IDictionary<Type, IDictionary<CultureInfo, string>> generatedTypes;
        private readonly Stack<IIndentation> indentations;

        public CodeBuilder()
        {
            this.buffer = new StringBuilder();
            this.references = new HashSet<string>();
            this.generatedTypes = new Dictionary<Type, IDictionary<CultureInfo, string>>();
            this.indentations = new Stack<IIndentation>();
        }

        public string SourceCode => this.buffer.ToString();

        public IEnumerable<string> References => references;

        public IEnumerable<IImplementationType> Implementations
        {
            get
            {
                foreach (KeyValuePair<Type, IDictionary<CultureInfo, string>> typeCulture in this.generatedTypes)
                    foreach (KeyValuePair<CultureInfo, string> cultureImplementation in typeCulture.Value)
                        yield return new PrvImplementationType
                        {
                            InterfaceType = typeCulture.Key,
                            Culture = cultureImplementation.Key,
                            ImplementationTypeName = cultureImplementation.Value,
                        };
            }
        }

        public void SetImplementation(IImplementationType generatedType)
        {
            if (!this.generatedTypes.TryGetValue(generatedType.InterfaceType
                , out IDictionary<CultureInfo, string> cultureImplementationTypes))
            {
                cultureImplementationTypes = new Dictionary<CultureInfo, string>();
                this.generatedTypes.Add(generatedType.InterfaceType, cultureImplementationTypes);
            }

            cultureImplementationTypes[generatedType.Culture] = generatedType.ImplementationTypeName;
        }

        public ICodeBuilder Append(string source)
        {
            source.ShouldBeNotNull(nameof(source));

            for (int i = 0, n = this.indentations.Count; i < n; i++)
                this.buffer.Append('\t');

            this.buffer.AppendLine(source);
            return this;
        }

        public ICodeBuilder Deindent(IIndentation token)
        {
            IIndentation topToken;

            if (this.indentations.Count == 0)
                throw new DeindentationUnderflowLocalizationException();

            topToken = this.indentations.Pop();

            if (ReferenceEquals(topToken, token))
                return this;

            throw new WrongDeindentationLocalizationException(token);
        }

        public ICodeBuilder Indent(out IIndentation token)
        {
            token = new PrvIndentation();
            this.indentations.Push(token);

            return this;
        }

        public void SetReference(string reference)
        {
            string referencePath;

            referencePath = reference.AsFullPathReference();
            this.references.Add(referencePath);
        }

        private class PrvIndentation : IIndentation { }

        private class PrvImplementationType : IImplementationType
        {
            public Type InterfaceType { get; set; }
            public CultureInfo Culture { get; set; }
            public string ImplementationTypeName { get; set; }
        }
    }
}
