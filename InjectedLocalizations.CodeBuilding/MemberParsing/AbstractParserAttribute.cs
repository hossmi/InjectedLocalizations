using System;

namespace InjectedLocalizations.MemberParsing
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class AbstractParserAttribute : Attribute
    {
        public AbstractParserAttribute(IMemberParser parser)
        {
            this.Parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }
        public IMemberParser Parser { get; }
    }
}
