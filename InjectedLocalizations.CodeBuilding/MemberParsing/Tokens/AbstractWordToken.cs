namespace InjectedLocalizations.MemberParsing.Tokens
{
    public abstract class AbstractWordToken : IPrintableToken
    {
        public AbstractWordToken(string value)
        {
            this.Value = value;
        }

        public string Value { get; }
    }
}
