using System.Globalization;

namespace InjectedLocalizations.Models
{
    public class ParametrizedCultureSomeLocalizations : ISomeLocalizations
    {
        public ParametrizedCultureSomeLocalizations(CultureInfo culture)
        {
            this.Culture = culture;
        }

        public CultureInfo Culture { get; }

        public string User_0_is_valid(string name) => $"User {name} is valid";
    }
}
