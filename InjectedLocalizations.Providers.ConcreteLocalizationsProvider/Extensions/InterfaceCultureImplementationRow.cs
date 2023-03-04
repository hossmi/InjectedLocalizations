using System;
using System.Globalization;

namespace InjectedLocalizations.Extensions
{
    public class InterfaceCultureImplementationRow
    {
        public Type Interface { get; set; }
        public Type Implementation { get; set; }
        public CultureInfo Culture { get; set; }
    }
}
