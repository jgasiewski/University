using System.Collections.Generic;
using UnityEngine;

public class Expression
{
    Token operand;
    Expression left;
    Expression right;

    public Expression(Token operand, Expression left, Expression right)
    {
        this.operand = operand;
        this.left = left;
        this.right = right;
    }

    public float Execute(Dictionary<string, float> variables)
    {
        switch (operand.type)
        {
            case TokenType.AddOp:
                return Add(variables);
            case TokenType.Bool:
                return Bool();
            case TokenType.EqOp:
                return Eq(variables);
            case TokenType.Float:
                return Value();
            case TokenType.Int:
                return Value();
            case TokenType.MulOp:
                return Mul(variables);
            case TokenType.RelOp:
                return Rel(variables);
            case TokenType.Sign:
                return Add(variables);
            case TokenType.VarName:
                return Var(variables);
            default:
                Debug.LogError("Expression cannot be parsed.");
                return 0;
        }
    }

    float Add(Dictionary<string, float> variables)
    {
        switch (operand.value)
        {
            case "+":
                if (left != null) return left.Execute(variables) + right.Execute(variables);
                else return right.Execute(variables);
            case "-":
                if (left != null) return left.Execute(variables) - right.Execute(variables);
                else return -right.Execute(variables);
            case "or":
                if ((left.Execute(variables) != 0) || (right.Execute(variables) != 0)) return 1;
                else return 0;
            default:
                Debug.LogError("Expression cannot be parsed.");
                return 0;
        }
    }

    float Bool()
    {
        if (operand.value == "true") return 1;
        else return 0;
    }

    float Eq(Dictionary<string, float> variables)
    {
        string varName = left.operand.value;

        if (variables.ContainsKey(varName))
        {
            variables[varName] = right.Execute(variables);
            return 1;
        }
        else
        {
            Debug.LogError("Variable " + varName + " doesn't exist.");
            return 0;
        }
    }

    float Value()
    {
        return float.Parse(operand.value);
    }

    float Mul(Dictionary<string, float> variables)
    {
        switch (operand.value)
        {
            case "*":
                return left.Execute(variables) * right.Execute(variables);
            case "/":
                return left.Execute(variables) / right.Execute(variables);
            case "and":
                if ((left.Execute(variables) != 0) && (right.Execute(variables) != 0)) return 1;
                else return 0;
            default:
                Debug.LogError("Expression cannot be parsed.");
                return 0;
        }
    }

    float Rel(Dictionary<string, float> variables)
    {
        switch (operand.value)
        {
            case "<=":
                if (left.Execute(variables) <= right.Execute(variables)) return 1;
                else return 0;
            case "<":
                if (left.Execute(variables) < right.Execute(variables)) return 1;
                else return 0;
            case "==":
                if (left.Execute(variables) == right.Execute(variables)) return 1;
                else return 0;
            case "!=":
                if (left.Execute(variables) != right.Execute(variables)) return 1;
                else return 0;
            case ">=":
                if (left.Execute(variables) >= right.Execute(variables)) return 1;
                else return 0;
            case ">":
                if (left.Execute(variables) <= right.Execute(variables)) return 1;
                else return 0;
            default:
                Debug.LogError("Expression cannot be parsed.");
                return 0;
        }
    }

    float Var(Dictionary<string, float> variables)
    {
        string varName = operand.value;

        if (variables.ContainsKey(varName))
        {
            return variables[varName];
        }
        else
        {
            Debug.LogError("Variable " + varName + " doesn't exist.");
            return 0;
        }
    }
}
