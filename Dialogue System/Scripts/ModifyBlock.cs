using System.Collections.Generic;

public class ModifyBlock : Block
{
    Expression expr;

    public ModifyBlock(Expression expr)
    {
        this.expr = expr;
    }

    public override Block Enter(Dictionary<string, float> variables)
    {
        expr.Execute(variables);
        if (successor != null) return successor.Enter(variables);
        else return null;
    }
}
