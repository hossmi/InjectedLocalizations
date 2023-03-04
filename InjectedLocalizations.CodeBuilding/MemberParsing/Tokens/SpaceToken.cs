namespace InjectedLocalizations.MemberParsing.Tokens
{
    public class SpaceToken : AbstractSeparatorToken
    {
        public SpaceToken(int length)
        {
            this.Length = length;
            this.Value = new string(' ', length);
        }

        public static SpaceToken Simple { get; } = new SpaceToken(1);

        public int Length { get; }
        public override string Value { get; }
    }
}
