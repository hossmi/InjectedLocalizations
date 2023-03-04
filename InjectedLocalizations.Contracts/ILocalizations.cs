using System.Globalization;

namespace InjectedLocalizations
{
    public interface ILocalizations
    {
        CultureInfo Culture { get; }
    }
}
