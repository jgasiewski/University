using System;
using System.Collections.Generic;
using UnityEngine;

public class Parser
{
    Scanner scanner;

    IEnumerator<Token> nextToken;
    Token token;

    Dictionary<string, float> variables;
    Dictionary<string, Block> entryPoints;
    List<KeyValuePair<DialogueBlock, string>> transitionBindings;
    List<Block> blocks;

    Expression expr;
    bool once = false;

    Block hookBlock;
    Block nextToBind;

    public Parser()
    {
        scanner = new Scanner();
        variables = new Dictionary<string, float>();
        entryPoints = new Dictionary<string, Block>();
        transitionBindings = new List<KeyValuePair<DialogueBlock, string>>();
        blocks = new List<Block>();
    }

    public DialogueObject Compile(string fileName)
    {
        scanner.QueueFile(fileName);
        entryPoints.Add("end", null);

        nextToken = scanner.GetEnumerator();
        token = nextToken.Current;

        try
        {
            NextToken();
            while(token.type != TokenType.Info)
            {
                ScriptPart();
            }

            BindTransitions();

            return new DialogueObject(variables, entryPoints, blocks);
        }
        catch(Exception e)
        {
            Debug.LogError(scanner.FileName+" "+ scanner.TokenRow+":"+ scanner.TokenColumn +" " + e.Message);
            Clear();
            return null;
        }
    }

    void BindTransitions()
    {
        foreach (KeyValuePair<DialogueBlock, string> pair in transitionBindings)
        {
            Block block;

            if(entryPoints.TryGetValue(pair.Value, out block))
            {
                pair.Key.BindTransition(block);
            }
            else
            {
                throw new Exception("Hook " + pair.Value + " not found");
            }
        }
    }

    void Clear()
    {
        scanner.Reset();
        variables.Clear();
        entryPoints.Clear();
        transitionBindings.Clear();
        blocks.Clear();
        once = false;
        hookBlock = nextToBind = null;
    }

    void ScriptPart()
    {
        switch (token.type)
        {
            case TokenType.Include:
                NextToken();
                Include();
                break;
            case TokenType.Var:
                NextToken();
                VarDeclaration();
                break;
            case TokenType.HookBegin:
                string name = token.value;
                NextToken();
                Hook(name);
                break;
            default:
                throw new Exception("Unexpected token " + token.ToString() + ".");
        }
    }

    void Include()
    {
        if (token.type == TokenType.String)
        {
            scanner.QueueFile(token.value);
            NextToken();
        }
        else
        {
            throw new Exception("Unexpected token " + token.ToString() + ", expected string instead.");
        }
    }

    void VarDeclaration()
    {
        if (token.type == TokenType.VarName)
        {
            variables.Add(token.value, 0);
            NextToken();
        }
        else
        {
            throw new Exception("Unexpected token " + token.ToString() + ", expected variable name instead.");
        }
    }

    void Block()
    {
        switch (token.type)
        {
            case TokenType.CondBeginTag:
                NextToken();
                Condition();
                break;
            case TokenType.String:
                string text = token.value;
                NextToken();
                DialogueBlock(text);
                break;
            case TokenType.OptionsBeginTag:
                NextToken();
                OptionsBlock(false, false);
                break;
            case TokenType.LeftCurlyBracket:
                NextToken();
                ListBlock(false, false);
                break;
            case TokenType.RandomTag:
                NextToken();
                ListStyle(true, false);
                break;
            case TokenType.LoopedTag:
                NextToken();
                ListStyle(false, true);
                break;
            case TokenType.ModifyTag:
                NextToken();
                ModifyBlock();
                break;
        }
    }

    void ListBlock(bool isRandom, bool  isLooped)
    {
        if (token.type == TokenType.OptionsBeginTag)
        {
            NextToken();
            OptionsBlock(isRandom, isLooped);
        }
        else
        {
            ListBlock block = new ListBlock(false, isRandom, isLooped, 0);
            BindLastWith(block);
            blocks.Add(block);

            do
            {
                nextToBind = null;

                int next = blocks.Count;

                Block();

                if (next < blocks.Count)
                {
                    block.AddBlock(null, blocks[next]);
                }
                else
                {
                    throw new Exception("Non-empty block expected.");
                }
            } while (token.type == TokenType.ListSeparator && NextToken());

            if (token.type != TokenType.RightCurlyBracket)
            {
                throw new Exception("Unexpected token " + token.ToString() + ", expected } instead.");
            }

            nextToBind = block;

            NextToken();
        }

        Block();
    }

    void ListStyle(bool isRandom, bool isLooped)
    {
        switch (token.type)
        {
            case TokenType.RandomTag:
                NextToken();
                ListStyle(true, isLooped);
                break;
            case TokenType.LoopedTag:
                NextToken();
                ListStyle(isRandom, true);
                break;
            case TokenType.LeftCurlyBracket:
                NextToken();
                ListBlock(isRandom, isLooped);
                break;
            default:
                throw new Exception("Unexpected token " + token.ToString() + ".");
        }
    }

    void DialogueBlock(string text)
    {
        DialogueBlock block = new DialogueBlock(text, expr, once);
        BindLastWith(block);

        expr = null;
        once = false;
        TransitionBranch(block);

        nextToBind = block;
        blocks.Add(block);

        Block();
    }

    void TransitionBranch(DialogueBlock block)
    {
        if(token.type != TokenType.TransitionTag)
        {
            return;
        }
        else
        {
            NextToken();
            if (token.type == TokenType.CondBeginTag)
            {
                NextToken();
                Condition();
            }

            if (token.type == TokenType.VarName)
            {
                block.BindTransitionCond(expr, once);
                expr = null;
                once = false;
                transitionBindings.Add(new KeyValuePair<DialogueBlock, string>(block, token.value));
                NextToken();
            }
            else
            {
                throw new Exception("Unexpected token " + token.ToString() + ". Expected variable name or \"end\" instead.");
            }
        }
    }

    void Condition()
    {
        expr = Expr();

        if (token.type != TokenType.CondEndTag)
        {
            throw new Exception("Unexpected token " + token.ToString() + ". Expected \"then\" keywoard instead.");
        }
        NextToken();

        if (token.type == TokenType.OnceTag)
        {
            once = true;
            NextToken();
        }

        if(token.type == TokenType.String)
        {
            string text = token.value;
            NextToken();
            DialogueBlock(text);
        }
        else if(token.type != TokenType.VarName)
        {
            throw new Exception("Unexpected token " + token.ToString() + ".");
        }
    }

    Expression Expr()
    {
        Expression left = SimpExpr();
        if(token.type == TokenType.RelOp)
        {
            Token op = token;
            NextToken();
            Expression right = SimpExpr();

            return new Expression(op, left, right);
        }
        else
        {
            return left;
        }
    }

    Expression SimpExpr()
    {
        Expression left = Term();
        if (token.type == TokenType.AddOp || token.type == TokenType.Sign)
        {
            Token op = token;
            NextToken();
            Expression right = Term();

            return new Expression(op, left, right);
        }
        else
        {
            return left;
        }
    }

    Expression Term()
    {
        Expression left = Factor();
        if (token.type == TokenType.MulOp)
        {
            Token op = token;
            NextToken();
            Expression right = Factor();

            return new Expression(op, left, right);
        }
        else
        {
            return left;
        }
    }

    Expression Factor()
    {
        if(token.type == TokenType.Int || token.type == TokenType.Float
            || token.type == TokenType.Bool || token.type == TokenType.VarName)
        {
            Expression result = new Expression(token, null, null);
            NextToken();
            return result;
        }
        else if (token.type == TokenType.LeftBracket)
        {
            NextToken();
            Expression result = Expr();

            if (token.type != TokenType.RightBracket)
            {
                throw new Exception("Unexpected token " + token.ToString() + ". Expected \")\" instead.");
            }

            NextToken();

            return result;
        }
        else
        {
            throw new Exception("Unexpected token " + token.ToString() + ".");
        }
    }

    void OptionsBlock(bool isRandom, bool isLooped)
    {
        int defaultVal = 0;

        if (token.type == TokenType.DefaultTag)
        {
            NextToken();
            if (token.type == TokenType.Int)
            {
                defaultVal = int.Parse(token.value);
                NextToken();
            }
            else
            {
                throw new Exception("Unexpected token " + token.ToString() + ", expected int instead.");
            }
        }
        if (token.type == TokenType.LeftCurlyBracket)
        {
            NextToken();
            OptionsList(isRandom, isLooped, defaultVal);

            if (token.type != TokenType.RightCurlyBracket)
            {
                throw new Exception("Unexpected token " + token.ToString() + ", expected } instead.");
            }

            NextToken();
        }
        else
        {
            throw new Exception("Unexpected token " + token.ToString() + ", expected { instead.");
        }

        Block();
    }

    void OptionsList(bool isRandom, bool isLooped, int defaultVal)
    {
        ListBlock block = new ListBlock(true, isRandom, isLooped, defaultVal);
        BindLastWith(block);
        blocks.Add(block);

        if (token.type != TokenType.OptionTag)
        {
            throw new Exception("Required non-empty choice list.");
        }

        do
        {
            nextToBind = null;
            NextToken();

            if (token.type != TokenType.String)
            {
                throw new Exception("Unexpected token " + token.ToString() + ", expected string instead.");
            }

            string text = token.value;
            NextToken();

            if (token.type == TokenType.LeftCurlyBracket)
            {
                NextToken();

                int next = blocks.Count;

                Block();

                if (next < blocks.Count)
                {
                    block.AddBlock(text, blocks[next]);
                }
                else
                {
                    throw new Exception("Block cannot be empty.");
                }

                if (token.type != TokenType.RightCurlyBracket)
                {
                    throw new Exception("Unexpected token " + token.ToString() + ", expected } instead.");
                }

                NextToken();
            }
            else
            {
                throw new Exception("Unexpected token " + token.ToString() + ", expected { instead.");
            }
        }
        while (token.type == TokenType.OptionTag);

        nextToBind = block;
    }

    void ModifyBlock()
    {
        Token left = token;
        Token op;

        if (token.type == TokenType.VarName)
        {
            NextToken();
            if (token.type == TokenType.EqOp)
            {
                op = token;

                NextToken();
                Expression expr = new Expression(op, new Expression(left, null, null), Expr());

                ModifyBlock block = new ModifyBlock(expr);
                BindLastWith(block);
                blocks.Add(block);
                nextToBind = block;

                Block();
            }
            else
            {
                throw new Exception("Unexpected token " + token.ToString() + ", expected \"=\" instead.");
            }
        }
        else
        {
            throw new Exception("Unexpected token " + token.ToString() + ", expected var name instead.");
        }
    }

    void Hook(string name)
    {
        int next = blocks.Count;

        Block();

        if (next < blocks.Count)
        {
            hookBlock = blocks[next];
            BindLastWith(null);
        }

        HookEnd(name);
    }

    void HookEnd(string name)
    {
        if (token.type == TokenType.HookEnd)
        {
            if(token.value == name) NextToken();
            else throw new Exception("Unexpected end of hook " + token.value);
        }
        else
        {
            throw new Exception("Unexpected " + token.ToString() + ", expected hook end instead.");
        }

        if (hookBlock != null)
        {
            if(entryPoints.ContainsKey(name))
            {
                throw new Exception("Hook " + name + " already exists.");
            }
            else
            {
                entryPoints.Add(name, hookBlock);
                hookBlock = null;
            }
        }
        else
        {
            throw new Exception("Hook " + name + " cannot be empty.");
        }
    }

    void BindLastWith (Block block)
    {
        if (nextToBind != null)
        {
            nextToBind.SetSuccessor(block);
            nextToBind = null;
        }
    }

    bool NextToken()
    {
        bool result = nextToken.MoveNext();
        token = nextToken.Current;
        return result;
    }
}
