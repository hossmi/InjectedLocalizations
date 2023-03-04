using FluentAssertions;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.MemberParsing.Tokens;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;

namespace InjectedLocalizations.InterpolationTests
{
    public class AsInterpolatedStringTests
    {
        [Fact]
        public void Can_get_interpolated_string_from_The_user_0_with_code_1_is_forbiden()
        {
            IParsedMember tokens;
            IReadOnlyList<string> result;

            tokens = Build.ParsedMember(typeof(IValidLocalizations)
                .GetMethod(nameof(IValidLocalizations.The_user_0_with_code_1_is_forbiden))
                , new CapitalWordToken("The")
                , SpaceToken.Simple
                , new WordToken("user")
                , SpaceToken.Simple
                , 0
                , SpaceToken.Simple
                , new WordToken("with")
                , SpaceToken.Simple
                , new WordToken("code")
                , SpaceToken.Simple
                , 1
                , SpaceToken.Simple
                , new WordToken("is")
                , SpaceToken.Simple
                , new WordToken("forbiden")
                , EndToken.End);

            result = tokens.AsInterpolatedString().ToList();

            result[0].Should().Be("The");
            result[1].Should().Be(" ");
            result[2].Should().Be("user");
            result[3].Should().Be(" ");
            result[4].Should().Be("{");
            result[5].Should().Be("userName");
            result[6].Should().Be("}");
            result[7].Should().Be(" ");
            result[8].Should().Be("with");
            result[9].Should().Be(" ");
            result[10].Should().Be("code");
            result[11].Should().Be(" ");
            result[12].Should().Be("{");
            result[13].Should().Be("userId");
            result[14].Should().Be("}");
            result[15].Should().Be(" ");
            result[16].Should().Be("is");
            result[17].Should().Be(" ");
            result[18].Should().Be("forbiden");
            result.Should().HaveCount(19);
        }

        [Fact]
        public void Can_interpolate_Some_property_ISample_property()
        {
            IParsedMember tokens;
            IReadOnlyList<string> result;

            tokens = Build.ParsedMember(typeof(ISample).GetProperty(nameof(ISample.Some_property))
                , new CapitalWordToken("Some")
                , SpaceToken.Simple
                , new WordToken("property")
                , EndToken.End);

            result = tokens.AsInterpolatedString().ToList();

            result[0].Should().Be("Some");
            result[1].Should().Be(" ");
            result[2].Should().Be("property");
            result.Should().HaveCount(3);
        }

        [Fact]
        public void Can_get_There_are_0_objects_ISample_property()
        {
            IParsedMember tokens;
            IReadOnlyList<string> result;

            tokens = Build.ParsedMember(typeof(ISample).GetProperty(nameof(ISample.There_are_0_objects))
                , new CapitalWordToken("There")
                , SpaceToken.Simple
                , new WordToken("are")
                , SpaceToken.Simple
                , new NumberToken("0")
                , SpaceToken.Simple
                , new WordToken("objects")
                , EndToken.End);

            result = tokens.AsInterpolatedString().ToList();

            result[0].Should().Be("There");
            result[1].Should().Be(" ");
            result[2].Should().Be("are");
            result[3].Should().Be(" ");
            result[4].Should().Be("0");
            result[5].Should().Be(" ");
            result[6].Should().Be("objects");
            result.Should().HaveCount(7);
        }

        [Fact]
        public void Can_get_X0_0_1_2x_ISample_property()
        {
            IParsedMember tokens;
            IReadOnlyList<string> result;

            tokens = Build.ParsedMember(typeof(ISample).GetProperty(nameof(ISample.X0_0_1_2x))
                , new CapitalWordToken("X")
                , SpaceToken.Simple
                , new NumberToken("0")
                , SpaceToken.Simple
                , new NumberToken("0")
                , SpaceToken.Simple
                , new NumberToken("1")
                , SpaceToken.Simple
                , new NumberToken("2")
                , SpaceToken.Simple
                , new WordToken("x")
                , EndToken.End);

            result = tokens.AsInterpolatedString().ToList();

            result[0].Should().Be("X");
            result[1].Should().Be(" ");
            result[2].Should().Be("0");
            result[3].Should().Be(" ");
            result[4].Should().Be("0");
            result[5].Should().Be(" ");
            result[6].Should().Be("1");
            result[7].Should().Be(" ");
            result[8].Should().Be("2");
            result[9].Should().Be(" ");
            result[10].Should().Be("x");
            result.Should().HaveCount(11);
        }
    }
}
