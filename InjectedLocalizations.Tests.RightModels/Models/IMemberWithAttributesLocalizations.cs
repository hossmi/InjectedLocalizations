namespace InjectedLocalizations.Models
{
    public interface IMemberWithAttributesLocalizations : ILocalizations
    {
        [Question]
        string The_file_0_already_exists_Do_you_want_to_overwrite_it(string fileName);

        [Exclamation]
        string An_unexpected_error_has_taken_place { get; }

        [Ellipsis]
        string Please_wait__the_file_is_still_downloading { get; }

        [Ellipsis]
        string PleaseWait_TheFileIsStillDownloading { get; }
    }
}
