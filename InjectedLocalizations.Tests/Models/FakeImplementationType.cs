using System.Globalization;
using InjectedLocalizations.Building;

namespace InjectedLocalizations.Models
{
    public class FakeImplementationType : IImplementationType
    {
        public Type InterfaceType { get; set; }
        public CultureInfo Culture { get; set; }
        public string ImplementationTypeName { get; set; }
    }
}
