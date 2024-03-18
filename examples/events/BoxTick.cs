using Godot;
using Godot.Collections;
using Miniscript;
using Miniscript.example.vector;
using System;

namespace Miniscript.example.events;

public partial class BoxTick : CsgBox3D
{
    [Export] public ScriptEngine scriptEngine;

    [Export] public float rayLength = 1000f;

    int ticks = 0;

    public override void _PhysicsProcess(double delta)
    {
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
        else if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Left)
        {
            var dict = shootRay(eventMouseButton);
            if (dict.Count == 0) return;

            var map = new ValMap();
            map["pos"] = new Vec3((Vector3)dict["position"]);
            scriptEngine.Publish("click", map);
        }
    }

    private Dictionary shootRay(InputEventMouseButton eventMouseButton)
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var camera3D = GetViewport().GetCamera3D();
        var from = camera3D.ProjectRayOrigin(eventMouseButton.Position);
        var to = from + camera3D.ProjectRayNormal(eventMouseButton.Position) * rayLength;
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var intersection = spaceState.IntersectRay(query);

        return intersection;
    }
}
