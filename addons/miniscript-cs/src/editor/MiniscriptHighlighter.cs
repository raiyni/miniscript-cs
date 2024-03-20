using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class MiniscriptHighlighter : CodeHighlighter
{
    private static List<string> NUMERIC_INTRINSICS = new List<string>()
    {
        "abs", "acos", "asin", "atan", "ceil", "char", "cos",
        "floor", "log", "round", "rnd", "pi", "sign", "sin",
        "sqrt", "str", "tan"
    };

    private static List<string> STRING_INTRINSICS = new List<string>()
    {
        "indexOf", "insert", "len", "val", "code", "remove",
        "lower", "upper", "replace", "split"
    };

    private static List<string> LIST_MAP_INTRINSICS = new List<string>()
    {
        "hasIndex", "indexOf", "insert", "join", "push", "pop",
        "pull", "indexes", "values", "len", "sum", "sort",
        "shuffle", "remove", "range"
    };

    private static List<string> GLOBAL_INTRINSICS = new List<string>()
    {
        "print", "time", "wait"
    };

    private static List<string> VARIABLES = new List<string>()
    {
        "true", "false", "locals", "globals"
    };

    private static List<string> TOKENS = new List<string>()
    {
        "function", "while", "end", "if", "for", "then", 
        "new", "else", "break", "continue"
    };

    [Export] public Color TokenColor = Colors.Salmon;
    [Export] public Color IntrinsicsColor = Colors.PaleGreen;
    [Export] public Color VariablesColor = Colors.DodgerBlue.Lightened(0.35f);
    [Export] public Color CommentColor = Colors.Gray.Darkened(0.25f);
    [Export] public Color StringColor = Colors.Gold.Lightened(0.3f);
    

    public MiniscriptHighlighter()
    {
        TOKENS.ForEach(s => AddKeywordColor(s, TokenColor));

        NUMERIC_INTRINSICS
            .Concat(GLOBAL_INTRINSICS)
            .Concat(STRING_INTRINSICS)
            .Concat(LIST_MAP_INTRINSICS)
            .ToList()
            .ForEach(s => AddKeywordColor(s, IntrinsicsColor));

        VARIABLES.ForEach(s => AddKeywordColor(s, VariablesColor));

        AddColorRegion("//", "", CommentColor);
        AddColorRegion("\"", "\"", StringColor); 
    }
}
