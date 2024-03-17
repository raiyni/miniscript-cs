using Godot;
using Miniscript;
using System;

public partial class Box : CsgBox3D
{
    [Export] public ScriptEngine scriptEngine;

    public override void _PhysicsProcess(double delta)
    {
        scriptEngine.Publish("tick");
        scriptEngine.Run();
    }
}
