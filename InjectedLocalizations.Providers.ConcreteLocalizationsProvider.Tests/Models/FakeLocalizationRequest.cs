using System.Globalization;
using InjectedLocalizations.Building;

namespace InjectedLocalizations.Models
{
    public class FakeLocalizationRequest : ILocalizationRequest
    {
        public Type InterfaceType { get; set; }
        public CultureInfo Culture { get; set; }
    }
}
