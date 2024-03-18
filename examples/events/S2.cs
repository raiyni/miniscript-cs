using Godot;
using Miniscript;
using System;

public partial class S2 : ScriptEngine
{
    [ExportGroup("Features")]
    [Export] public bool allowMoving = true;
}
