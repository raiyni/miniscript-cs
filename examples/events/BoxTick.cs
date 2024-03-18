using Godot;
using Miniscript;
using System;

namespace Miniscript.example.events;

public partial class BoxTick : CsgBox3D
{
    [Export] public ScriptEngine scriptEngine;

    int ticks = 0;

    public override void _PhysicsProcess(double delta)
    {

        if (Input.IsActionJustPressed("mouse_1"))
        {
            scriptEngine.Publish("click");
        }

        ticks++;
        var map = new ValMap();
        if (ticks % 30 == 0)
        {
            map["pass"] = ValNumber.Truth(true);
            ticks = 0;
        }
        else
        {
            map["pass"] = ValNumber.Truth(false);
        }

        scriptEngine.Publish("tick", map);

        if (scriptEngine.Running())
            scriptEngine.Run();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey e && e.Keycode == Key.F10 && e.Pressed)
        {
            var map = new ValMap();
            map["msg"] = new ValString(scriptEngine.Script);
            scriptEngine.Publish("echo", map);
        }
    }
}
