using System;

namespace InjectedLocalizations.Building
{
    public interface IError
    {
        string Message { get; }
        Type Type { get; }
    }
}