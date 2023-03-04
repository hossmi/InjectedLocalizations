using System;

namespace InjectedLocalizations.MemberParsing.Tokens
{
    public class NewLineToken : AbstractSeparatorToken
    {
        private NewLineToken() { }
        public static NewLineToken NewLine { get; } = new NewLineToken();
        public override string Value => Environment.NewLine;
    }
}
