using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP
{
    public abstract class TokenBase
    {
        private int endIndex;
        private int length;
        private string rawText;

        public TokenType TokenType { get; private set; }
        public int StartIndex { get; set; }
        public int EndIndex
        {
            get { return endIndex; }
            set
            {
                endIndex = value;
                length = endIndex + 1 - StartIndex;
            }
        }

        public int Length
        {
            get { return length; }
            set
            {
                length = value;
                endIndex = StartIndex + value - 1;
            }
        }
        public string RawText
        {
            get { return rawText; }
            set
            {
                rawText = value;
                length = (string.IsNullOrEmpty(value) ? 0 : rawText.Length);
            }
        }
        public TokenBase(TokenType tokenType, int startIndex, string rawText)
        {
            this.TokenType = tokenType;
            StartIndex = startIndex;
            this.RawText = rawText;
        }
        public override string ToString() => RawText;

        public string Normalize()
        {
            return RawText.ToLower().Trim();
        }

    }

    public class WhiteSpaceToken : TokenBase
    {
        public WhiteSpaceToken(int startIndex, string rawText) : base(TokenType.WhiteSpace, startIndex, rawText) { }

    }
    public class TextToken : TokenBase
    {
        public TextToken(int startIndex, string rawText) : base(TokenType.Text, startIndex, rawText) { }
 

    }

    public class TokenCollectionTests
    {

        public static void Run()
        {
            //TODO: assert each NLPTokenCollection.ToString()==input;
            TokenCollection a = "a";
            TokenCollection _a = " a";
            TokenCollection __a = "  a";

            TokenCollection a_ = "a ";
            TokenCollection _a_ = " a ";
            TokenCollection __a_ = "  a ";
            TokenCollection __a__ = "  a  ";

            TokenCollection ab = "ab";
            TokenCollection _ab = " ab";
            TokenCollection __ab = "  ab";

            TokenCollection ab_ = "ab ";
            TokenCollection _ab_ = " ab ";
            TokenCollection __ab_ = "  ab ";
            TokenCollection __ab__ = "  ab  ";

            TokenCollection abc = "abc";
            TokenCollection _abc = " abc";
            TokenCollection __abc = "  abc";

            TokenCollection abc_ = "abc ";
            TokenCollection _abc_ = " abc ";
            TokenCollection __abc_ = "  abc ";
            TokenCollection __abc__ = "  abc  ";


            TokenCollection abc_d = "abc d";
            TokenCollection _abc_d = " abc d";
            TokenCollection __abc_d = "  abc d";

            TokenCollection abc_d_ = "abc d ";
            TokenCollection _abc_d_ = " abc d ";
            TokenCollection __abc_d_ = "  abc d ";
            TokenCollection __abc_d__ = "  abc d  ";

        }
    }


    public class TokenCollection :IEnumerable<TokenBase>
    {
        public List<TokenBase> Tokens { get; set; }
        public int Count => Tokens.Count;

        public bool HasText => Tokens.Any(x => x.TokenType == TokenType.Text);

        public IEnumerable<TextToken> TextTokens => Tokens.Where(x => x.TokenType == TokenType.Text).Cast<TextToken>();
        public TokenCollection()
        {
            this.Tokens = new List<TokenBase>();
        }

        public static TokenCollection Parse(string input)
        {
            var result = new TokenCollection();
            var reader = new TokenReader(input);
            while (reader.ReadToken())
            {
                result.Tokens.Add(reader.CurrentToken);
            }

            return result;
        }
        public override string ToString()
        {
            return string.Join("", Tokens);
        }

        public IEnumerator<TokenBase> GetEnumerator() => Tokens.GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public static implicit operator TokenCollection(string value) => TokenCollection.Parse(value);
    }

    public class TokenReader
    {
        private string input;
        private int Position;
        private int startPosition;
        private TokenType tokenType;
        private StringBuilder buffer;
        Func<char, bool> readFilter;
        public TokenReader(string input)
        {
            this.input = input;
            this.tokenType = TokenType.Unknown;
            this.buffer = new StringBuilder();
        }


        public TokenBase CurrentToken { get; internal set; }

        public bool ReadToken()
        {
            ClearState();

            if (Position < input.Length)
            {
                if (char.IsWhiteSpace(input[Position]))
                {
                    ReadWhiteSpace();
                }
                else
                {
                    ReadText();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ClearState()
        {
            CurrentToken = null;
            this.tokenType = TokenType.Unknown;
            buffer.Clear();
            readFilter = null;
        }


        private string ReadRaw(Func<char, bool> filter)
        {
            readFilter = filter;
            var rawText = ReadRaw();
            return rawText;
        }

        private string ReadRaw()
        {
            startPosition = Position;
            for (; Position < input.Length && readFilter(input[Position]); Position++)
            {
                buffer.Append(input[Position]);
            }
            return buffer.ToString();
        }


        private void ReadText()
        {
            CurrentToken = new TextToken(startPosition, ReadRaw((c) => !char.IsWhiteSpace(c)));
        }

        private void ReadWhiteSpace()
        {
            CurrentToken = new WhiteSpaceToken(startPosition, ReadRaw((c) => char.IsWhiteSpace(c)));
        }

        public static TokenCollection Tokenize(string input)
        {
            return TokenCollection.Parse(input);

        }
    }

    public static class CharExtensions
    {
        public static bool IsPuncation(this char c) => Char.IsPunctuation(c);
    }

    public class PunctuationTokens
    {
        public const char Period = '.';
        public const char Comma = ',';
        public const char QuestionMark = '?';
        public const char SingleQuote = '\'';
        public const char DoubleQuote = '"';
        public const char SemiColon = ';';
        public const char Colon = ':';
        public const char Exclamation = ':';
        public const string AllTokensString = ".,?'\";:!";
        public static readonly char[] AllTokens = new[] { Period, Comma, QuestionMark, SingleQuote, DoubleQuote, SemiColon, Colon, Exclamation };
        public static bool IsToken(char c) => Array.IndexOf(AllTokens, c) > -1;
    }


    public class GroupingTokens
    {
        public const char BeginParenthesis = '(';
        public const char EndParenthesis = ')';
        public const char BeginSquareBracket = '[';
        public const char EndSquareBracket = ']';
        public const char BeginBracket = '{';
        public const char EndBracket = '}';
        public const char BeginAngleBracket = '<';
        public const char EndAngleBracket = '>';

        public const string AllTokensString = "()[]{}<>";
        public static readonly char[] AllTokens = AllTokensString.ToCharArray();
        public static bool IsToken(char c) => Array.IndexOf(AllTokens, c) > -1;
    }

    public class ArithmeticTokens
    {
        public const char Plus = '+';
        public const char Minus = '-';
        public const char Multiply = '*';
        public const char Multiply2 = 'x';
        public const char Multiply3 = 'X';
        public const char Divide = '/';
        public const char DivdeFlowEven = '\\';

        public const string AllTokensString = "+-*xX/\\";
        public static readonly char[] AllTokens = AllTokensString.ToCharArray();
        public static bool IsToken(char c) => Array.IndexOf(AllTokens, c) > -1;


    }

    public class OperatorTokens
    {
        public const char OpAnd = '&';
        public const char OpOR = '|';
        public const char OpNot = '~';
        public const char OpXor = '^';
        public const char OpRange = ':';
        public const char Fact = '!';
        public const char Inverse = '`';
        public const char OpMoney = '$';
        public const char OpPercent = '%';
        public const string AllTokensString = "&|~^:!`$%";
        public static readonly char[] AllTokens = AllTokensString.ToCharArray();
        public static bool IsToken(char c) => Array.IndexOf(AllTokens, c) > -1;
    }

    public class EqualityTokens
    {
        public const char LessThan = '<';
        public const char GreaterThan = '>';
        public const char Equal = '=';
        public const string AllTokensString = "<>=";
        public static readonly char[] AllTokens = AllTokensString.ToCharArray();
        public static bool IsToken(char c) => Array.IndexOf(AllTokens, c) > -1;
    }

    public class WhiteSpaceTokens
    {
        public const char Space = ' ';
        public const char CarriageReturn = '\r';
        public const char LineFeed = '\n';
        public const char Tab = '\t';
        public const string AllTokensString = " \r\n\t";
        public static readonly char[] AllTokens = AllTokensString.ToCharArray();
        public static bool IsToken(char c) => Array.IndexOf(AllTokens, c) > -1;
    }

    public class AlphaUpperTokens
    {
        public const char A = 'A';
        public const char B = 'B';
        public const char C = 'C';
        public const char D = 'D';
        public const char E = 'E';
        public const char F = 'F';
        public const char G = 'G';
        public const char H = 'H';
        public const char I = 'I';
        public const char J = 'J';
        public const char K = 'K';
        public const char L = 'L';
        public const char M = 'M';
        public const char N = 'N';
        public const char O = 'O';
        public const char P = 'P';
        public const char Q = 'Q';
        public const char R = 'R';
        public const char S = 'S';
        public const char T = 'T';
        public const char U = 'U';
        public const char V = 'V';
        public const char W = 'W';
        public const char X = 'X';
        public const char Y = 'Y';
        public const char Z = 'Z';
        public const string AllTokensString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly char[] AllTokens = AllTokensString.ToCharArray();
        public static bool IsToken(char c) => Array.IndexOf(AllTokens, c) > -1;
    }
    public class AlphaLowerTokens
    {
        public const char a = 'a';
        public const char b = 'b';
        public const char c = 'c';
        public const char d = 'd';
        public const char e = 'e';
        public const char f = 'f';
        public const char g = 'g';
        public const char h = 'h';
        public const char i = 'i';
        public const char j = 'j';
        public const char k = 'k';
        public const char l = 'l';
        public const char m = 'm';
        public const char n = 'n';
        public const char o = 'o';
        public const char p = 'p';
        public const char q = 'q';
        public const char r = 'r';
        public const char s = 's';
        public const char t = 't';
        public const char u = 'u';
        public const char v = 'v';
        public const char w = 'w';
        public const char x = 'x';
        public const char y = 'y';
        public const char z = 'z';
        public const string AllTokensString = "abcdefghijklmnopqrstuvwxyz";
        public static readonly char[] AllTokens = AllTokensString.ToCharArray();
        public static bool IsToken(char c) => Array.IndexOf(AllTokens, c) > -1;
    }
    public class DigitTokens
    {
        public const char Zero = '0';
        public const char One = '1';
        public const char Two = '2';
        public const char Three = '3';
        public const char Four = '4';
        public const char Five = '5';
        public const char Six = '6';
        public const char Seven = '7';
        public const char Eight = '8';
        public const char Nine = '9';
        public const string AllTokensString = "0123456789";
        public static readonly char[] AllTokens = AllTokensString.ToCharArray();
        public static bool IsToken(char c) => Array.IndexOf(AllTokens, c) > -1;


    }


    //word, symbol, punctuation, special character, white space
    public enum TokenType
    {
        Unknown = 0,
        Text = 1,
        WhiteSpace = 2,
        Punctations = 3,
    }

}
