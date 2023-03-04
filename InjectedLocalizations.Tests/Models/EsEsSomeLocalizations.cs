using System.Globalization;

namespace InjectedLocalizations.Models
{
    public class EsEsSomeLocalizations : ISomeLocalizations
    {
        public CultureInfo Culture { get; } = new CultureInfo("es-ES");

        public string User_0_is_valid(string name) => $"El usuario {name} es correcto";
    }
}
