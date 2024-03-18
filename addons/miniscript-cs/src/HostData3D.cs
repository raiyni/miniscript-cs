using Godot;

namespace Miniscript;

public class HostData3D : HostData
{
    public new readonly Node3D Node;
    public HostData3D(Node n, ScriptEngine e) : base((Node3D)n, e)
    {
        Node = (Node3D)n;
    }
}