using System.Globalization;

namespace InjectedLocalizations.Models
{
    public class EnglishEmptyLocalizations : IEmptyLocalizations
    {
        public CultureInfo Culture { get; } = new CultureInfo("en-US");
    }
}
