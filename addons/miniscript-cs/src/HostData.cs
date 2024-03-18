using Godot;

namespace Miniscript;

public class HostData
{
    public Node Node { get; }
    public Node2D Node2D { get => (Node2D)Node; }
    public Node3D Node3D { get => (Node3D)Node; }

    public ScriptEngine Engine { get; }

    public HostData(Node n, ScriptEngine e)
    {
        Node = n;
        Engine = e;
    }
}