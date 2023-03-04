namespace InjectedLocalizations.MemberParsing.Tokens
{
    public class SymbolToken : AbstractWordToken
    {
        public SymbolToken(string symbol) : base(symbol) { }
        public static SymbolToken Dot { get; } = new SymbolToken(".");
        public static SymbolToken Comma { get; } = new SymbolToken(",");
    }
}
