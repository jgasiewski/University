using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Scanner : IEnumerable<Token>
{
    Reader reader;
    Dictionary<int, Token> keywords;

    IEnumerator<char> nextChar;
    char c;

    int tokenRow, tokenColumn;
    string fileName;

    public int TokenRow
    {
        get
        {
            return tokenRow;
        }
    }

    public int TokenColumn
    {
        get
        {
            return tokenColumn;
        }
    }

    public string FileName
    {
        get
        {
            return fileName;
        }
    }

    public Scanner()
    {
        reader = new Reader();
        keywords = new Dictionary<int, Token>();
        InitDictionary();
    }

    void InitDictionary()
    {
        keywords.Add(new Hash("true"), new Token("true", TokenType.Bool));
        keywords.Add(new Hash("false"), new Token("false", TokenType.Bool));
        keywords.Add(new Hash("var"), new Token("var", TokenType.Var));
        keywords.Add(new Hash("include"), new Token("include", TokenType.Include));
        keywords.Add(new Hash("random"), new Token("random", TokenType.RandomTag));
        keywords.Add(new Hash("looped"), new Token("looped", TokenType.LoopedTag));
        keywords.Add(new Hash("if"), new Token("if", TokenType.CondBeginTag));
        keywords.Add(new Hash("then"), new Token("then", TokenType.CondEndTag));
        keywords.Add(new Hash("once"), new Token("once", TokenType.OnceTag));
        keywords.Add(new Hash("choose"), new Token("choose", TokenType.OptionsBeginTag));
        keywords.Add(new Hash("default"), new Token("default", TokenType.DefaultTag));
        keywords.Add(new Hash("modify"), new Token("modify", TokenType.ModifyTag));
        keywords.Add(new Hash("or"), new Token("or", TokenType.AddOp));
        keywords.Add(new Hash("and"), new Token("and", TokenType.MulOp));
        keywords.Add(new Hash("end"), new Token("end", TokenType.VarName));
    }

    public void QueueFile(string fileName)
    {
        reader.QueueFile(fileName);
    }

    public void Reset()
    {
        nextChar = null;
        reader.Reset();
    }

    public IEnumerator<Token> GetEnumerator()
    {
        nextChar = reader.GetEnumerator();
        NextChar();

        while (nextChar.Current != (char)0)
        {
            fileName = reader.FileName;
            tokenRow = reader.RowNumber;
            tokenColumn = reader.ColumnNumber;

            if (char.IsWhiteSpace(c))
            {
                NextChar();
            }
            else if (c == '"') yield return StringToken();
            else if (char.IsNumber(c))
            {
                yield return NumberToken();
            }
            else if (char.IsLetter(c))
            {
                yield return WordToken();
            }
            else if (c == '[')
            {
                yield return BlockToken();
            }
            else
            {
                Token t = SymbolToken();
                if (t != null) yield return t;
                else continue;
            }
        }

        yield return new Token("EOF", TokenType.Info);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        yield return GetEnumerator().Current;
    }

    Token ErrorToken(string error)
    {
        return new Token(reader.FileName + " " + reader.RowNumber + ":" + reader.ColumnNumber + " " +
                    error, TokenType.Error);
    }

    Token StringToken()
    {
        StringBuilder builder = new StringBuilder();
        bool escape = false;

        while (NextChar())
        {
            if (escape)
            {
                escape = false;
                if (c == 'n') builder.Append('\n');
                else if (c == '"') builder.Append('\"');
                else if (c == '\\') builder.Append('\\');
                else return ErrorToken("Unexpected escape character \"\\\".");
            }
            else if (c == '"')
            {
                NextChar();
                return new Token(builder.ToString(), TokenType.String);
            }
            else if (c == (char)0)
            {
                return ErrorToken("Unexpected end of file. Expected: \".");
            }
            else if (char.IsWhiteSpace(c))
            {
                if (c == '\n') builder.Append(' ');
                else if (builder.Length > 0 && !char.IsWhiteSpace(builder[builder.Length - 1]))
                {
                    builder.Append(c);
                }
            }
            else if (c == '\\')
            {
                escape = true;
            }
            else builder.Append(c);
        }
        return ErrorToken("Unexpected end of file. Expected: \".");
    }

    Token NumberToken()
    {
        StringBuilder builder = new StringBuilder();

        bool isFloat = false;

        do
        {
            if (char.IsDigit(c))
            {
                builder.Append(c);
            }
            else if (c == '.')
            {
                if (isFloat)
                {
                    NextChar();
                    return ErrorToken("Unexpected character: \".\"");
                }
                else
                {
                    builder.Append(c);
                    isFloat = true;
                }
            }
            else
            {
                break;
            }
        }
        while (NextChar());

        if (builder.Length > 1 && (builder[0] == '0' && builder[1] != '.'))
        {
            return ErrorToken("Incorrect number format: " + builder.ToString());
        }
        else if (isFloat)
        {
            return new Token(builder.ToString(), TokenType.Float);
        }
        else
        {
            return new Token(builder.ToString(), TokenType.Int);
        }
    }

    Token WordToken()
    {
        StringBuilder builder = new StringBuilder();
        Hash hash = new Hash();

        while (char.IsLetter(c) || char.IsNumber(c))
        {
            builder.Append(c);
            hash.Append(c);
            NextChar();
        }

        string s = builder.ToString();
        Token t = null;

        if (keywords.TryGetValue(hash, out t))
        {
            if (t.value == s)
            {
                return t;
            }
        }

        return new Token(s, TokenType.VarName);
    }

    Token BlockToken()
    {
        NextChar();

        bool blockEnd = false;
        if (c == '/')
        {
            blockEnd = true;
            NextChar();
        }

        Token t = WordToken();

        if (t.type != TokenType.VarName)
        {
            return ErrorToken("Unexpected block name: " + t.value + ".");
        }

        if (c == ']')
        {
            NextChar();
            if (blockEnd)
            {
                t.type = TokenType.HookEnd;
            }
            else
            {
                t.type = TokenType.HookBegin;
            }
            return t;
        }
        else
        {
            return ErrorToken("Mismatched brackets, expected ].");
        }
    }

    Token SymbolToken()
    {
        switch (c)
        {
            case '-':
                return MinusToken();
            case '=':
                return EqualToken();
            case '{':
                NextChar();
                return new Token("{", TokenType.LeftCurlyBracket);
            case '}':
                NextChar();
                return new Token("}", TokenType.RightCurlyBracket);
            case '(':
                NextChar();
                return new Token("(", TokenType.LeftBracket);
            case ')':
                NextChar();
                return new Token(")", TokenType.RightBracket);
            case '+':
                NextChar();
                return new Token("+", TokenType.Sign);
            case '*':
                NextChar();
                return new Token("*", TokenType.MulOp);
            case '/':
                return SlashToken();
            case '<':
                return LessThanToken();
            case '>':
                return MoreThanToken();
            case '!':
                return NegationToken();
            case ',':
                NextChar();
                return new Token(",", TokenType.ListSeparator);
            default:
                char old = c;
                NextChar();
                return ErrorToken("Unexpected symbol: \"" + old + "\".");
        }
    }

    Token MinusToken()
    {
        NextChar();
        switch (c)
        {
            case '>':
                NextChar();
                return new Token("->", TokenType.OptionTag);
            default:
                return new Token("-", TokenType.Sign);
        }
    }

    Token EqualToken()
    {
        NextChar();
        switch (c)
        {
            case '>':
                NextChar();
                return new Token("=>", TokenType.TransitionTag);
            case '=':
                NextChar();
                return new Token("==", TokenType.RelOp);
            default:
                return new Token("=", TokenType.EqOp);
        }
    }

    Token SlashToken()
    {
        NextChar();
        switch (c)
        {
            case '*':
                bool asterisk = false;
                while (NextChar())
                {
                    if (c == '*')
                    {
                        asterisk = true;
                    }
                    else if(c == '/' && asterisk)
                    {
                        NextChar();
                        return null;
                    }
                    else
                    {
                        asterisk = false;
                    }
                }
                return ErrorToken("Unexpected end of file. Expected */.");
            case '/':
                while (NextChar())
                {
                    if (c == '\n')
                    {
                        NextChar();
                        return null;
                    }
                }
                return new Token("EOF", TokenType.Info);
            default:
                return new Token("/", TokenType.MulOp);
        }
    }

    Token LessThanToken()
    {
        NextChar();
        switch (c)
        {
            case '=':
                NextChar();
                return new Token("<=", TokenType.RelOp);
            default:
                return new Token("<", TokenType.RelOp);
        }
    }

    Token MoreThanToken()
    {
        NextChar();
        switch (c)
        {
            case '=':
                NextChar();
                return new Token(">=", TokenType.RelOp);
            default:
                return new Token(">", TokenType.RelOp);
        }
    }

    Token NegationToken()
    {
        NextChar();
        switch (c)
        {
            case '=':
                NextChar();
                return new Token("!=", TokenType.RelOp);
            default:
                return ErrorToken("Unexpected symbol: \"!\".");
        }
    }

    bool NextChar()
    {
        bool result = nextChar.MoveNext();
        c = nextChar.Current;
        return result;
    }
}
