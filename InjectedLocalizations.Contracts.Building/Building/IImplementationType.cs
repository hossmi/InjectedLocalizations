using System;

namespace InjectedLocalizations.Building
{
    public interface IImplementationType : IImplementation
    {
        Type InterfaceType { get; }
    }
}