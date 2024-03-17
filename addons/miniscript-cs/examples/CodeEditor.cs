using Godot;
using Miniscript;
using System;
using System.Collections.Generic;
using System.Linq;

namespace code;

public partial class CodeEditor : CanvasLayer
{
    [Export] public ScriptEngine scriptEngine;
    [Export] public CodeEdit codeEdit;

    [Export] public Button compileButton;
    [Export] public Button runButton;

    public override void _Ready()
    {
        compileButton.Pressed += Compile;
        runButton.Pressed += Run;

        codeEdit.Text = scriptEngine.Script;
    }

    public void Compile()
    {
        // Recompile happens on set
        scriptEngine.Script = codeEdit.Text;
    }

    public void Run()
    {
        Compile();
        scriptEngine.Run();
    }
}
