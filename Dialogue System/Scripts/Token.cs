public class Token
{
    public string value;
    public TokenType type;

    public Token(string value, TokenType type)
    {
        this.value = value;
        this.type = type;
    }

    public override string ToString()
    {
        if (value == null) return type.ToString();
        else return value;
    }
}

public enum TokenType
{
    Error,
    Info,
    String,
    Int,
    Float,
    Bool,
    Var,
    VarName,
    Include,
    HookBegin,
    HookEnd,
    OptionTag,
    LeftCurlyBracket,
    RightCurlyBracket,
    ListSeparator,
    TransitionTag,
    RandomTag,
    LoopedTag,
    CondBeginTag,
    CondEndTag,
    OnceTag,
    OptionsBeginTag,
    DefaultTag,
    ModifyTag,
    LeftBracket,
    RightBracket,
    EqOp,
    RelOp,
    AddOp,
    MulOp,
    Sign
}
