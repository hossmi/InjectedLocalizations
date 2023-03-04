using System;
using System.Collections.Generic;
using System.Globalization;
using InjectedLocalizations.Building;
using InjectedLocalizations.Configuration;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.Extensions;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.Providers
{
    public class ConcreteLocalizationsProvider : ILocalizationsProvider
    {
        private readonly IDictionary<Type, IDictionary<CultureInfo, Type>> implementations;

        public ConcreteLocalizationsProvider(ILocalizationsAssembliesConfiguration assembliesConfiguration)
        {
            this.implementations = assembliesConfiguration
                .ShouldBeNotNull(nameof(assembliesConfiguration))
                .BuildMasterDictionary();
        }

        public bool CanIssueToWriters => false;
        public bool CanWriteFromIssuers => false;

        public void RegisterWriter(ILocalizationsProvider provider) => throw new NotIssuerProviderLocalizationException(this);

        public void TryBuildLocalizationFor(ILocalizationRequest request, ICodeBuilder builder)
        {
            if (this.implementations.TryGetValue(request.InterfaceType, out var cultures))
            {
                if (cultures.TryGetValue(request.Culture, out Type concreteType))
                {
                    builder.SetImplementation(new PrvImplementationType
                    {
                        Culture = request.Culture,
                        InterfaceType = request.InterfaceType,
                        ImplementationTypeName = concreteType.FullName,
                    });

                    builder.SetReference(concreteType.Assembly.Location);
                }
            }
        }

        public void Write(ILocalizationResponse response) => throw new NotWritableProviderLocalizationException(this);

        private class PrvImplementationType : IImplementationType
        {
            public CultureInfo Culture { get; set; }
            public Type InterfaceType { get; set; }
            public string ImplementationTypeName { get; set; }
        }
    }
}
