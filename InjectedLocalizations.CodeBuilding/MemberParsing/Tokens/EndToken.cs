namespace InjectedLocalizations.MemberParsing.Tokens
{
    public class EndToken : IToken
    {
        private EndToken() { }

        public static EndToken End { get; } = new EndToken();
    }
}
