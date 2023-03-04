using System;
using System.Reflection;

namespace InjectedLocalizations.Providers
{
    public interface IAssemblyBuilder
    {
        Assembly Build(Type interfaceType);
    }
}
