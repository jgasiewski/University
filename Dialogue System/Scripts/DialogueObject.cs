using System.Collections.Generic;

public class DialogueObject
{
    public Dictionary<string, float> variables;
    public Dictionary<string, Block> entryPoints;
    public List<Block> blocks;
    Block curr;

    public DialogueObject(Dictionary<string, float> variables, Dictionary<string, Block> entryPoints, List<Block> blocks)
    {
        this.variables = variables;
        this.entryPoints = entryPoints;
        this.blocks = blocks;
        curr = null;
    }

    public Block Start(string hookName)
    {
        curr = null;

        entryPoints.TryGetValue(hookName, out curr);
        curr = curr.Enter(variables);

        return curr;
    }

    public Block Next()
    {
        curr = curr.Continue(variables);
        return curr;
    }
}
