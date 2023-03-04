using System;
using System.Reflection;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Providers;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Building
{
    public class DefaultAssemblyBuilder : IAssemblyBuilder
    {
        private readonly ILocalizationsCultureConfiguration cultureConfiguration;
        private readonly ILocalizationsProviderConfiguration providersConfiguration;

        public DefaultAssemblyBuilder(ILocalizationsCultureConfiguration cultureConfiguration
            , ILocalizationsProviderConfiguration providersConfiguration)
        {
            this.cultureConfiguration = cultureConfiguration.ShouldBeNotNull(nameof(cultureConfiguration));
            this.providersConfiguration = providersConfiguration.ShouldBeNotNull(nameof(providersConfiguration));
        }

        public Assembly Build(Type interfaceType)
        {
            Assembly assemblyResult;
            CodeBuilder builder;

            builder = new CodeBuilder();
            builder.BuildSource(interfaceType, this.providersConfiguration, this.cultureConfiguration);
            assemblyResult = builder.CompileAssembly();

            return assemblyResult;
        }

    }
}
