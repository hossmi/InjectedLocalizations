using System.Globalization;

namespace InjectedLocalizations.Models
{
    public class EnUsSomeLocalizations : ISomeLocalizations
    {
        public CultureInfo Culture { get; } = new CultureInfo("en-US");

        public string User_0_is_valid(string name) => $"The user {name} is valid";
    }
}
