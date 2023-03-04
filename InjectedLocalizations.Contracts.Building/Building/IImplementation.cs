using System.Globalization;

namespace InjectedLocalizations.Building
{
    public interface IImplementation
    {
        CultureInfo Culture { get; }
        string ImplementationTypeName { get; }
    }
}