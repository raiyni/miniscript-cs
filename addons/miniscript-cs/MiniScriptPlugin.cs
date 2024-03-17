using Godot;

namespace Miniscript;

#if TOOLS
[Tool]
public partial class MiniScriptPlugin : EditorPlugin
{
    public override void _EnterTree()
    {
        var script = GD.Load<Godot.Script>("res://addons/miniscript-cs/src/ScriptEngine.cs");
        var texture = GD.Load<Texture2D>("res://addons/miniscript-cs/icon.png");
        AddCustomType("ScriptEngine", "Node", script, texture);
    }

    public override void _ExitTree()
    {
        RemoveCustomType("ScriptEngine");
    }
}
#endif