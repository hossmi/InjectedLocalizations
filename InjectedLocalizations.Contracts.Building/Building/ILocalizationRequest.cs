using System;
using System.Globalization;

namespace InjectedLocalizations.Building
{
    public interface ILocalizationRequest
    {
        Type InterfaceType { get; }
        CultureInfo Culture { get; }
    }
}
