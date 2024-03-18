using Godot;

namespace Miniscript;

public class HostData2D : HostData
{
    public new readonly Node2D Node;
    public HostData2D(Node n, ScriptEngine e) : base((Node2D)n, e)
    {
        Node = (Node2D)n;
    }
}