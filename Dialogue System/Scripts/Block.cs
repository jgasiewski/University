using System.Collections.Generic;

public abstract class Block
{
    public Block successor = null;

    public virtual void SetSuccessor(Block block)
    {
        successor = block;
    }

    public virtual Block Enter(Dictionary<string, float> variables)
    {
        return this;
    }

    public virtual Block Continue(Dictionary<string, float> variables)
    {
        if (successor != null) return successor.Enter(variables);
        else return null;
    }
}
