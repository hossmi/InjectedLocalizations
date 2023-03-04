using FluentAssertions;
using InjectedLocalizations.MemberParsing;
using InjectedLocalizations.Models;
using InjectedLocalizations.Providers;
using JimenaTools.Extensions.Strings;

namespace InjectedLocalizations.InterpolationTests
{
    public class MemberAttributeTests
    {
        [Fact]
        public void Can_get_interpolated_question()
        {
            typeof(IMemberWithAttributesLocalizations)
                .GetMethod(nameof(IMemberWithAttributesLocalizations.The_file_0_already_exists_Do_you_want_to_overwrite_it))
                .AsParsedMember()
                .AsInterpolatedString()
                .AsJoined()
                .Should()
                .Be("The file {fileName} already exists, do you want to overwrite it?");
        }

        [Fact]
        public void Can_get_interpolated_exclamation()
        {
            typeof(IMemberWithAttributesLocalizations)
                .GetProperty(nameof(IMemberWithAttributesLocalizations.An_unexpected_error_has_taken_place))
                .AsParsedMember()
                .AsInterpolatedString()
                .AsJoined()
                .Should()
                .Be("An unexpected error has taken place!");
        }

        [Fact]
        public void Can_get_interpolated_ellipsis_1()
        {
            typeof(IMemberWithAttributesLocalizations)
                .GetProperty(nameof(IMemberWithAttributesLocalizations.Please_wait__the_file_is_still_downloading))
                .AsParsedMember()
                .AsInterpolatedString()
                .AsJoined()
                .Should()
                .Be("Please wait, the file is still downloading...");
        }

        [Fact]
        public void Can_get_interpolated_ellipsis_2()
        {
            typeof(IMemberWithAttributesLocalizations)
                .GetProperty(nameof(IMemberWithAttributesLocalizations.PleaseWait_TheFileIsStillDownloading))
                .AsParsedMember()
                .AsInterpolatedString()
                .AsJoined()
                .Should()
                .Be("Please wait, the file is still downloading...");
        }
    }
}
