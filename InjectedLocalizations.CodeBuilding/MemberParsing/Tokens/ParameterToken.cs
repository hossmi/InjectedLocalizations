using System.Reflection;

namespace InjectedLocalizations.MemberParsing.Tokens
{
    public class ParameterToken : IPrintableToken
    {
        public ParameterToken(ParameterInfo parameter)
        {
            this.Parameter = parameter;
        }
        public string Value => this.Parameter.Name;
        public ParameterInfo Parameter { get; }
    }
}
