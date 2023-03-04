using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.MemberParsing
{
    public abstract class AbstractMemberParser : IMemberParser
    {
        protected delegate IEnumerable<IToken> InnerParseDelegate(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters);
        protected const char UNDERSCORE = '_';

        public IParsedMember Parse(MemberInfo member)
        {
            ParameterInfo[] parameters;
            IReadOnlyCollection<IToken> tokens;

            member.ShouldBeNotNull(nameof(member));

            if (member is MethodInfo method)
                parameters = method.GetParameters();
            else
                parameters = new ParameterInfo[0];

            using (IEnumerator<char> cursor = member.Name.GetEnumerator())
            {
                cursor.MoveNext();
                tokens = BeginParse(cursor, parameters).ToList();
            }

            return new PrvParsedMember
            {
                Tokens = tokens,
                Member = member,
            };
        }

        protected abstract IEnumerable<IToken> BeginParse(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters);

        private class PrvParsedMember : IParsedMember
        {
            public MemberInfo Member { get; set; }
            public int Count => this.Tokens.Count;
            public IReadOnlyCollection<IToken> Tokens { get; set; }
            public IEnumerator<IToken> GetEnumerator() => this.Tokens.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => this.Tokens.GetEnumerator();
        }
    }
}
