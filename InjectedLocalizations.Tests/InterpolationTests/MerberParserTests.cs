using FluentAssertions;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.MemberParsing.Tokens;

namespace InjectedLocalizations.InterpolationTests
{
    public class MerberParserTests
    {
        [Fact]
        public void Some_property()
        {
            IMemberParser parser;
            IReadOnlyList<IToken> tokens;

            parser = new DefaultMemberParser();
            tokens = parser.Parse(this.GetType().GetMethod(nameof(Some_property))).ToList();

            tokens[0]
                .Should().BeOfType<CapitalWordToken>()
                .Which.Value.Should().Be("Some");

            tokens[1]
                .Should().BeOfType<SpaceToken>()
                .Which.Length.Should().Be(1);

            tokens[2]
                .Should().BeOfType<WordToken>()
                .Which.Value.Should().Be("property");

            tokens[3].Should().BeOfType<EndToken>();
            tokens.Count.Should().Be(4);
        }

        [Fact]
        public void There_are_0_objects()
        {
            IMemberParser parser;
            IReadOnlyList<IToken> tokens;

            parser = new DefaultMemberParser();
            tokens = parser.Parse(this.GetType().GetMethod(nameof(There_are_0_objects))).ToList();

            tokens[0]
                .Should().BeOfType<CapitalWordToken>()
                .Which.Value.Should().Be("There");

            tokens[1]
                .Should().BeOfType<SpaceToken>()
                .Which.Length.Should().Be(1);

            tokens[2]
                .Should().BeOfType<WordToken>()
                .Which.Value.Should().Be("are");

            tokens[3]
                .Should().BeOfType<SpaceToken>()
                .Which.Length.Should().Be(1);

            tokens[4]
                .Should().BeOfType<NumberToken>()
                .Which.Value.Should().Be("0");

            tokens[5]
                .Should().BeOfType<SpaceToken>()
                .Which.Length.Should().Be(1);

            tokens[6]
                .Should().BeOfType<WordToken>()
                .Which.Value.Should().Be("objects");

            tokens[7].Should().BeOfType<EndToken>();
            tokens.Count.Should().Be(8);
        }

        [Fact]
        public void X0_0_1_2x()
        {
            IMemberParser parser;
            IReadOnlyList<IToken> tokens;

            parser = new DefaultMemberParser();
            tokens = parser.Parse(this.GetType().GetMethod(nameof(X0_0_1_2x))).ToList();

            tokens[0].ShouldBeCapital("X");
            tokens[1].ShouldBeSpace();
            tokens[2].ShouldBeNumber("0");
            tokens[3].ShouldBeSpace();
            tokens[4].ShouldBeNumber("0");
            tokens[5].ShouldBeSpace();
            tokens[6].ShouldBeNumber("1");
            tokens[7].ShouldBeSpace();
            tokens[8].ShouldBeNumber("2");
            tokens[9].ShouldBeSpace();
            tokens[10].ShouldBeWord("x");
            tokens.ShouldHaveEndTokenAsFinalItem();
        }

        [Fact]
        public void _0_1_2_()
        {
            IMemberParser parser;
            IReadOnlyList<IToken> tokens;

            parser = new DefaultMemberParser();
            tokens = parser.Parse(this.GetType().GetMethod(nameof(_0_1_2_))).ToList();

            tokens[0].ShouldBeSpace();
            tokens[1].ShouldBeNumber("0");
            tokens[2].ShouldBeSpace();
            tokens[3].ShouldBeNumber("1");
            tokens[4].ShouldBeSpace();
            tokens[5].ShouldBeNumber("2");
            tokens[6].ShouldBeSpace();
            tokens.ShouldHaveEndTokenAsFinalItem();
        }

        [Fact]
        public void _0_1_2___()
        {
            IMemberParser parser;
            IReadOnlyList<IToken> tokens;

            parser = new DefaultMemberParser();
            tokens = parser.Parse(this.GetType().GetMethod(nameof(_0_1_2___))).ToList();

            tokens[0].ShouldBeSpace();
            tokens[1].ShouldBeNumber("0");
            tokens[2].ShouldBeSpace();
            tokens[3].ShouldBeNumber("1");
            tokens[4].ShouldBeSpace();
            tokens[5].ShouldBeNumber("2");
            tokens[6].ShouldBeSpace(3);
            tokens.ShouldHaveEndTokenAsFinalItem();
        }

        [Fact]
        public void Sample_property__with_comma___with_dot____and_line_break()
        {
            IMemberParser parser;
            IReadOnlyList<IToken> tokens;

            parser = new DefaultMemberParser();
            tokens = parser.Parse(this.GetType().GetMethod(nameof(Sample_property__with_comma___with_dot____and_line_break))).ToList();

            tokens[0].ShouldBeCapital("Sample");
            tokens[1].ShouldBeSpace();
            tokens[2].ShouldBeWord("property");
            tokens[3].ShouldBeComma();
            tokens[4].ShouldBeSpace();

            tokens[5].ShouldBeWord("with");
            tokens[6].ShouldBeSpace();
            tokens[7].ShouldBeWord("comma");
            tokens[8].ShouldBeDot();
            tokens[9].ShouldBeSpace();

            tokens[10].ShouldBeCapital("With");
            tokens[11].ShouldBeSpace();
            tokens[12].ShouldBeWord("dot");
            tokens[13].ShouldBeDot();
            tokens[14].ShouldBeNewLine();

            tokens[15].ShouldBeCapital("And");
            tokens[16].ShouldBeSpace();
            tokens[17].ShouldBeWord("line");
            tokens[18].ShouldBeSpace();
            tokens[19].ShouldBeWord("break");

            tokens.ShouldHaveEndTokenAsFinalItem();
        }
    }
}
