using System.Collections.Generic;
using UnityEngine;

public class DialogueBlock : Block
{
    Expression entryCond;
    bool once;
    bool accessed;

    string text;

    Expression transitionCond;
    bool transitionOnce;
    bool transitionAccessed;
    Block transitionBlock;

    public DialogueBlock(string text, Expression entryCond, bool once)
    {
        this.text = text;
        this.entryCond = entryCond;
        this.once = once;
        accessed = false;

        transitionCond = null;
        transitionOnce = false;
        transitionAccessed = false;
        transitionBlock = null;
    }

    public void BindTransitionCond(Expression entryCond, bool once)
    {
        transitionCond = entryCond;
        transitionOnce = once;
    }

    public void BindTransition(Block block)
    {
        transitionBlock = block;
    }

    public override Block Enter(Dictionary<string, float> variables)
    {
        if ((entryCond == null || (entryCond != null && entryCond.Execute(variables) != 0)) && 
            (!once || (once && ! accessed)))
        {
            accessed = true;
            return this;
        }
        else
        {
            return null;
        }
    }

    public override Block Continue(Dictionary<string, float> variables)
    {
        if (transitionCond != null && transitionCond.Execute(variables) != 0 
            && (!transitionOnce || (transitionOnce && !transitionAccessed)))
        {
            transitionAccessed = true;
            return transitionBlock;
        }
        else
        {
            if (successor != null) return successor.Enter(variables);
            else return null;
        }
    }

    public override string ToString()
    {
        return text;
    }
}
