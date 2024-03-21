using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
        "new", "else", "break", "continue", "and", "or", "not"
    };

    [Export] public Color TokenColor = Colors.Salmon;
    [Export] public Color IntrinsicsColor = Colors.PaleGreen;
    [Export] public Color VariablesColor = Colors.DodgerBlue.Lightened(0.35f);
    [Export] public Color CommentColor = Colors.Gray.Darkened(0.25f);
    [Export] public Color StringColor = Color.FromString("#eee8aa", Colors.Gold.Lightened(0.4f));
    [Export] public Color BracketColor = Colors.Gold;
    [Export] public Color OpColor = Colors.Salmon;

    Dictionary<int, int> colorRegionCache = new Dictionary<int, int>();
    Dictionary<string, Color> memberKeywordColors = new Dictionary<string, Color>();
    Dictionary<string, Color> keywordColors = new Dictionary<string, Color>();


    public MiniscriptHighlighter()
    {
        TOKENS.ForEach(s => keywordColors[s] = TokenColor);

        NUMERIC_INTRINSICS
            .Concat(GLOBAL_INTRINSICS)
            .Concat(STRING_INTRINSICS)
            .Concat(LIST_MAP_INTRINSICS)
            .ToList()
            .ForEach(s => keywordColors[s] = IntrinsicsColor);

        VARIABLES.ForEach(s => keywordColors[s] = VariablesColor);

        AddColorRegion("//", "", CommentColor);
        AddColorRegion("\"", "\"", StringColor);
    }

    public override void _ClearHighlightingCache()
    {
        colorRegionCache.Clear();
    }

    // Translated directly from https://github.com/godotengine/godot/blob/fe01776f05b1787b28b4a270d53037a3c25f4ca2/scene/resources/syntax_highlighter.cpp#L119
    public override Godot.Collections.Dictionary _GetLineSyntaxHighlighting(int p_line)
    {
        Godot.Collections.Dictionary color_map = new Godot.Collections.Dictionary();

        bool prev_is_char = false;
        bool prev_is_number = false;
        bool in_keyword = false;
        bool in_word = false;
        bool in_function_name = false;
        bool in_member_variable = false;
        bool is_hex_notation = false;
        Color keyword_color = new Color();
        Color color = new Color();

        var text_edit = GetTextEdit();
        var font_color = text_edit.GetThemeColor("font_color");

        colorRegionCache[p_line] = -1;
        int in_region = -1;
        if (p_line != 0)
        {
            int prev_region_line = p_line - 1;
            while (prev_region_line > 0 && colorRegionCache.ContainsKey(prev_region_line))
            {
                prev_region_line--;
            }
            for (int i = prev_region_line; i < p_line - 1; i++)
            {
                GetLineSyntaxHighlighting(i);
            }
            if (!colorRegionCache.ContainsKey(p_line - 1))
            {
                GetLineSyntaxHighlighting(p_line - 1);
            }
            in_region = colorRegionCache[p_line - 1];
        }

        var str = text_edit.GetLine(p_line);
        int line_length = str.Length;
        Color prev_color = new Color();

        if (in_region != -1 && str.Length == 0)
        {
            colorRegionCache[p_line] = in_region;
        }
        for (int j = 0; j < line_length; j++)
        {
            Godot.Collections.Dictionary highlighter_info = new Godot.Collections.Dictionary();


            color = font_color;
            bool is_char = !is_symbol(str[j]);
            bool is_a_symbol = is_symbol(str[j]);
            bool is_number = is_digit(str[j]);
            bool is_a_bracket = is_a_symbol ? is_bracket(str[j]) : false;
            bool is_a_op = is_a_symbol ? is_op(str[j]) : false;

            /* color regions */
            if (is_a_symbol || in_region != -1)
            {
                int from = j;

                if (in_region == -1)
                {
                    for (; from < line_length; from++)
                    {
                        if (str[from] == '\\')
                        {
                            from++;
                            continue;
                        }
                        break;
                    }
                }

                if (from != line_length)
                {
                    /* check if we are in entering a region */
                    if (in_region == -1)
                    {
                        int c = -1;
                        foreach (var key in ColorRegions.Keys)
                        {
                            c++;
                            /* check there is enough room */
                            int chars_left = line_length - from;
                            var split = key.AsString().Split(" ");
                            var start_key = split[0];
                            var end_key = split.Length == 1 ? "" : split[1];

                            int start_key_length = start_key.Length;
                            int end_key_length = end_key.Length;
                            if (chars_left < start_key_length)
                            {
                                continue;
                            }

                            /* search the line */
                            bool match = true;
                            for (int k = 0; k < start_key_length; k++)
                            {
                                if (start_key[k] != str[from + k])
                                {
                                    match = false;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                continue;
                            }

                            in_region = c;
                            from += start_key_length;

                            /* check if it's the whole line */
                            if (end_key_length == 0 || from + end_key_length > line_length)
                            {
                                if (from + end_key_length > line_length && (start_key == "\"" || start_key == "\'"))
                                {
                                    // If it's key length and there is a '\', don't skip to highlight esc chars.
                                    if (str.IndexOf("\\", from) >= 0)
                                    {
                                        break;
                                    }
                                }
                                prev_color = ColorRegions[key].AsColor();
                                highlighter_info["color"] = ColorRegions[key].AsColor();
                                color_map[j] = highlighter_info;

                                j = line_length;
                                // TODO: C# bindings don't expose line_only from what I can tell
                                // if (!ColorRegions[c].line_only)
                                // {
                                //     colorRegionCache[p_line] = c;
                                // }
                            }
                            break;
                        }

                        if (j == line_length)
                        {
                            continue;
                        }
                    }

                    /* if we are in one find the end key */
                    if (in_region != -1)
                    {
                        var key = ColorRegions.Keys.ElementAt(in_region).AsString();
                        var split = key.Split(" ");
                        var start_key = split[0];
                        var end_key = split.Length == 1 ? "" : split[1];

                        bool is_string = start_key == "\"" || start_key == "\'";

                        Color region_color = ColorRegions[key].AsColor();
                        prev_color = region_color;
                        highlighter_info["color"] = region_color;
                        color_map[j] = highlighter_info;

                        /* search the line */
                        int region_end_index = -1;
                        int end_key_length = end_key.Length;
                        for (; from < line_length; from++)
                        {
                            if (line_length - from < end_key_length)
                            {
                                // Don't break if '\' to highlight esc chars.
                                if (!is_string || str.IndexOf("\\", from) < 0)
                                {
                                    break;
                                }
                            }

                            if (!is_symbol(str[from]))
                            {
                                continue;
                            }

                            if (str[from] == '\\')
                            {
                                if (is_string)
                                {
                                    Godot.Collections.Dictionary escape_char_highlighter_info = new Godot.Collections.Dictionary();
                                    escape_char_highlighter_info["color"] = SymbolColor;
                                    color_map[from] = escape_char_highlighter_info;
                                }

                                from++;

                                if (is_string)
                                {
                                    Godot.Collections.Dictionary region_continue_highlighter_info = new Godot.Collections.Dictionary();
                                    prev_color = region_color;
                                    region_continue_highlighter_info["color"] = region_color;
                                    color_map[from + 1] = region_continue_highlighter_info;
                                }
                                continue;
                            }

                            region_end_index = from;
                            for (int k = 0; k < end_key_length; k++)
                            {
                                if (end_key[k] != str[from + k])
                                {
                                    region_end_index = -1;
                                    break;
                                }
                            }

                            if (region_end_index != -1)
                            {
                                break;
                            }
                        }

                        j = from + (end_key_length - 1);
                        if (region_end_index == -1)
                        {
                            colorRegionCache[p_line] = in_region;
                        }

                        in_region = -1;
                        prev_is_char = false;
                        prev_is_number = false;
                        continue;
                    }
                }
            }

            // Allow ABCDEF in hex notation.
            if (is_hex_notation && (is_hex_digit(str[j]) || is_number))
            {
                is_number = true;
            }
            else
            {
                is_hex_notation = false;
            }

            // Check for dot or underscore or 'x' for hex notation in floating point number or 'e' for scientific notation.
            if ((str[j] == '.' || str[j] == 'x' || str[j] == '_' || str[j] == 'f' || str[j] == 'e') && !in_word && prev_is_number && !is_number)
            {
                is_number = true;
                is_a_symbol = false;
                is_char = false;

                if (str[j] == 'x' && str[j - 1] == '0')
                {
                    is_hex_notation = true;
                }
            }

            if (!in_word && (is_ascii_char(str[j]) || is_underscore(str[j])) && !is_number)
            {
                in_word = true;
            }

            if ((in_keyword || in_word) && !is_hex_notation)
            {
                is_number = false;
            }

            if (is_a_symbol && str[j] != '.' && in_word)
            {
                in_word = false;
            }

            if (!is_char)
            {
                in_keyword = false;
            }

            if (!in_keyword && is_char && !prev_is_char)
            {
                int to = j;
                while (to < line_length && !is_symbol(str[to]))
                {
                    to++;
                }

                String word = str[j..to];
                Color col = new Color();
                if (keywordColors.ContainsKey(word))
                {
                    col = keywordColors[word];
                }
                else if (memberKeywordColors.ContainsKey(word))
                {
                    col = memberKeywordColors[word];
                    for (int k = j - 1; k >= 0; k--)
                    {
                        if (str[k] == '.')
                        {
                            col = new Color(); //member indexing not allowed
                            break;
                        }
                        else if (str[k] > 32)
                        {
                            break;
                        }
                    }
                }

                if (col != new Color())
                {
                    in_keyword = true;
                    keyword_color = col;
                }
            }

            if (!in_function_name && in_word && !in_keyword)
            {
                int k = j;
                while (k < line_length - 1 && !is_symbol(str[k]) && str[k] != '\t' && str[k] != ' ')
                {
                    k++;
                }

                // Check for space between name and bracket.
                while (k < line_length - 1 && (str[k] == '\t' || str[k] == ' '))
                {
                    k++;
                }

                if (str[k] == '(')
                {
                    in_function_name = true;
                }
            }

            if (!in_function_name && !in_member_variable && !in_keyword && !is_number && in_word)
            {
                int k = j;
                while (k > 0 && !is_symbol(str[k]) && str[k] != '\t' && str[k] != ' ')
                {
                    k--;
                }

                if (str[k] == '.')
                {
                    in_member_variable = true;
                }
            }

            if (is_a_symbol)
            {
                in_function_name = false;
                in_member_variable = false;
            }

            if (in_keyword)
            {
                color = keyword_color;
            }
            else if (in_member_variable)
            {
                color = MemberVariableColor;
            }
            else if (in_function_name)
            {
                color = FunctionColor;
            }
            else if (is_a_bracket)
            {
                color = BracketColor;
            }
            else if (is_a_op)
            {
                color = OpColor;
            }
            else if (is_a_symbol)
            {
                color = SymbolColor;
            }
            else if (is_number)
            {
                color = NumberColor;
            }

            prev_is_char = is_char;
            prev_is_number = is_number;

            if (color != prev_color)
            {
                prev_color = color;
                highlighter_info["color"] = color;
                color_map[j] = highlighter_info;
            }
        }

        return color_map;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool is_symbol(char c)
    {
        return c != '_' && ((c >= '!' && c <= '/') || (c >= ':' && c <= '@') || (c >= '[' && c <= '`') || (c >= '{' && c <= '~') || c == '\t' || c == ' ');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool is_digit(char c)
    {
        return (c >= '0' && c <= '9');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool is_hex_digit(char c)
    {
        return (is_digit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool is_ascii_char(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool is_underscore(char p_char)
    {
        return p_char == '_';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool is_bracket(char p_char)
    {
        return p_char == '[' || p_char == ']' || p_char == '(' || p_char == ')' || p_char == '{' || p_char == '}' ;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool is_op(char p_char)
    {
        return p_char == '+' || p_char == '-' || p_char == '*' || p_char == '/' || p_char == '%' || p_char == '^' ;
    }
}
