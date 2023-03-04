namespace InjectedLocalizations.MemberParsing.Tokens
{
    public abstract class AbstractSeparatorToken : IToken, IPrintableToken
    {
        public abstract string Value { get; }
    }
}
