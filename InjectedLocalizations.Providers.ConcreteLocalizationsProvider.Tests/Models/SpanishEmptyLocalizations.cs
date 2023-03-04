using System.Globalization;

namespace InjectedLocalizations.Models
{
    public class SpanishEmptyLocalizations : IEmptyLocalizations
    {
        public CultureInfo Culture { get; } = new CultureInfo("es-ES");
    }
}
