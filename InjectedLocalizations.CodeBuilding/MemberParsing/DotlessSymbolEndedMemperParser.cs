using System.Collections.Generic;
using System.Reflection;
using InjectedLocalizations.Exceptions;
using InjectedLocalizations.MemberParsing.Tokens;
using JimenaTools.Extensions.Validations;

namespace InjectedLocalizations.MemberParsing
{
    public class DotlessSymbolEndedMemperParser : AbstractMemberParser
    {
        private readonly string endingsymbol;

        public DotlessSymbolEndedMemperParser(string endingSymbol)
        {
            this.endingsymbol = endingSymbol.ShouldBeFilled(nameof(endingSymbol));
        }

        protected override IEnumerable<IToken> BeginParse(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters)
        {
            if (cursor.Current == UNDERSCORE)
                return BeginLeftSideSpace(cursor, parameters);

            if (char.IsUpper(cursor.Current))
                return BeginCapital(cursor, parameters);

            if (char.IsLower(cursor.Current))
                return BeginWord(cursor, parameters);

            if (char.IsDigit(cursor.Current))
                return BeginNumber(cursor, parameters);

            throw new UnexpectedCharacterParsingLocalizationException(cursor, "");
        }

        private IEnumerable<IToken> BeginLeftSideSpace(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters) => ContinueLeftSideSpace(cursor, parameters, 1);

        private IEnumerable<IToken> ContinueLeftSideSpace(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters, int length)
        {
            InnerParseDelegate continueWith;

            if (!cursor.MoveNext())
            {
                yield return new SymbolToken(this.endingsymbol);
                yield return EndToken.End;
                yield break;
            }

            if (cursor.Current == UNDERSCORE)
            {
                foreach (IToken token in ContinueLeftSideSpace(cursor, parameters, length + 1))
                    yield return token;

                yield break;
            }

            continueWith = (c, p) => throw new UnexpectedCharacterParsingLocalizationException(c, new string(UNDERSCORE, length));

            if (char.IsUpper(cursor.Current))
                continueWith = BeginCapital;

            if (char.IsLower(cursor.Current))
                continueWith = BeginWord;

            if (char.IsDigit(cursor.Current))
                continueWith = BeginNumber;

            foreach (IToken token in continueWith(cursor, parameters))
                yield return token;
        }

        private IEnumerable<IToken> BeginInnerOrRightSpace(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters) => ContinueInnerOrRightSpace(cursor, parameters, 1);

        private IEnumerable<IToken> ContinueInnerOrRightSpace(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters, int length)
        {
            InnerParseDelegate continueWith;

            if (!cursor.MoveNext())
            {
                yield return new SymbolToken(this.endingsymbol);
                yield return EndToken.End;
                yield break;
            }

            if (cursor.Current == UNDERSCORE)
            {
                foreach (IToken token in ContinueInnerOrRightSpace(cursor, parameters, length + 1))
                    yield return token;

                yield break;
            }

            continueWith = (c, p) => throw new UnexpectedCharacterParsingLocalizationException(c, new string(UNDERSCORE, length));

            if (char.IsUpper(cursor.Current))
            {
                yield return SymbolToken.Comma;
                yield return SpaceToken.Simple;
                continueWith = BeginWord;
            }

            if (char.IsLower(cursor.Current))
            {
                if (2 <= length)
                    yield return SymbolToken.Comma;

                yield return SpaceToken.Simple;
                continueWith = BeginWord;
            }

            if (char.IsDigit(cursor.Current))
            {
                if (2 <= length)
                    yield return SymbolToken.Comma;

                yield return SpaceToken.Simple;
                continueWith = BeginNumber;
            }

            foreach (IToken token in continueWith(cursor, parameters))
                yield return token;
        }

        private IEnumerable<IToken> BeginCapital(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters) => ContinueCapital(cursor, parameters, char.ToUpper(cursor.Current).ToString());

        private IEnumerable<IToken> ContinueCapital(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters, string buffer)
        {
            InnerParseDelegate continueWith;

            if (!cursor.MoveNext())
            {
                yield return new CapitalWordToken(buffer);
                yield return new SymbolToken(this.endingsymbol);
                yield return EndToken.End;
                yield break;
            }

            continueWith = (c, p) => throw new UnexpectedCharacterParsingLocalizationException(c, buffer);

            if (char.IsLower(cursor.Current))
            {
                foreach (IToken token in ContinueCapital(cursor, parameters, buffer + cursor.Current))
                    yield return token;

                yield break;
            }

            yield return new CapitalWordToken(buffer);

            if (cursor.Current == UNDERSCORE)
                continueWith = BeginInnerOrRightSpace;

            if (char.IsUpper(cursor.Current))
            {
                yield return SpaceToken.Simple;
                continueWith = BeginWord;
            }

            if (char.IsDigit(cursor.Current))
            {
                yield return SpaceToken.Simple;
                continueWith = BeginNumber;
            }

            foreach (IToken token in continueWith(cursor, parameters))
                yield return token;
        }

        private IEnumerable<IToken> BeginWord(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters) => ContinueWord(cursor, parameters, char.ToLower(cursor.Current).ToString());

        private IEnumerable<IToken> ContinueWord(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters, string buffer)
        {
            InnerParseDelegate continueWith;

            if (!cursor.MoveNext())
            {
                yield return new WordToken(buffer);
                yield return new SymbolToken(this.endingsymbol);
                yield return EndToken.End;
                yield break;
            }

            if (char.IsLower(cursor.Current))
            {
                foreach (IToken token in ContinueWord(cursor, parameters, buffer + cursor.Current))
                    yield return token;

                yield break;
            }

            continueWith = (c, p) => throw new UnexpectedCharacterParsingLocalizationException(c, buffer);
            yield return new WordToken(buffer);

            if (cursor.Current == UNDERSCORE)
                continueWith = BeginInnerOrRightSpace;

            if (char.IsUpper(cursor.Current))
            {
                yield return SpaceToken.Simple;
                continueWith = BeginWord;
            }

            if (char.IsDigit(cursor.Current))
            {
                yield return SpaceToken.Simple;
                continueWith = BeginNumber;
            }

            foreach (IToken token in continueWith(cursor, parameters))
                yield return token;
        }

        private IEnumerable<IToken> BeginNumber(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters) => ContinueNumber(cursor, parameters, cursor.Current.ToString());

        private IEnumerable<IToken> ContinueNumber(IEnumerator<char> cursor, IReadOnlyList<ParameterInfo> parameters, string buffer)
        {
            InnerParseDelegate continueWith;

            if (!cursor.MoveNext())
            {
                yield return parameters.TryGetParameterToken(buffer);

                yield return new SymbolToken(this.endingsymbol);
                yield return EndToken.End;
                yield break;
            }

            if (char.IsDigit(cursor.Current))
            {
                foreach (IToken token in ContinueNumber(cursor, parameters, buffer + cursor.Current))
                    yield return token;

                yield break;
            }

            continueWith = (c, p) => throw new UnexpectedCharacterParsingLocalizationException(c, buffer);

            yield return parameters.TryGetParameterToken(buffer);

            if (cursor.Current == UNDERSCORE)
                continueWith = BeginInnerOrRightSpace;

            if (char.IsUpper(cursor.Current))
            {
                yield return SymbolToken.Dot;
                yield return SpaceToken.Simple;
                continueWith = BeginCapital;
            }

            if (char.IsLower(cursor.Current))
            {
                yield return SpaceToken.Simple;
                continueWith = BeginWord;
            }

            foreach (IToken token in continueWith(cursor, parameters))
                yield return token;
        }
    }
}
