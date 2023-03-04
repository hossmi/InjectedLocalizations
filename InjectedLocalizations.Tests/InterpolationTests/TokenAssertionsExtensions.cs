using FluentAssertions;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.MemberParsing.Tokens;

namespace InjectedLocalizations.InterpolationTests
{
    public static class TokenAssertionsExtensions
    {
        public static void ShouldBeSpace(this IToken token, int length = 1)
        {
            token.Should().BeOfType<SpaceToken>()
                .Which.Length.Should().Be(length);
        }

        public static void ShouldBeCapital(this IToken token, string text)
        {
            token.Should().BeOfType<CapitalWordToken>()
                .Which.Value.Should().Be(text);
        }

        public static void ShouldBeWord(this IToken token, string text)
        {
            token.Should().BeOfType<WordToken>()
                .Which.Value.Should().Be(text);
        }

        public static void ShouldBeNumber(this IToken token, string text)
        {
            token.Should().BeOfType<NumberToken>()
                .Which.Value.Should().Be(text);
        }

        public static void ShouldBeComma(this IToken token) => token.ShouldBeSymbol(",");
        public static void ShouldBeDot(this IToken token) => token.ShouldBeSymbol(".");

        public static void ShouldBeSymbol(this IToken token, string text)
        {
            token.Should().BeOfType<SymbolToken>()
                .Which.Value.Should().Be(text);
        }

        public static void ShouldBeNewLine(this IToken token) => token.Should().BeOfType<NewLineToken>();

        public static void ShouldHaveEndTokenAsFinalItem(this IReadOnlyList<IToken> tokens)
        {
            tokens[tokens.Count - 1].Should().BeOfType<EndToken>();
        }
    }
}
