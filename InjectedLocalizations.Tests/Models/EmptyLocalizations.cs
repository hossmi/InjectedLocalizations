using System.Globalization;

namespace InjectedLocalizations.Models
{
    public class EmptyLocalizations : IEmptyLocalizations
    {
        public CultureInfo Culture { get; } = new CultureInfo("en-US");
    }
}
