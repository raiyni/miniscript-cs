using Godot;

namespace Miniscript;

public class HostData2D : HostData
{
    new public Node2D Node { get; }
    new public ScriptEngine Engine { get; }

    public HostData2D(Node n, ScriptEngine e)
    {
        Node = (Node2D)n;
        Engine = e;
    }

    public override Intrinsic.Result Move(TAC.Context context)
    {
        if (Engine.allowMoving)
        {
            var vec = Node.Position;
            vec.X = (float)context.GetVar("x").DoubleValue();
            vec.Y = (float)context.GetVar("y").DoubleValue();

            Node.Position = vec;
        }
        
        return Intrinsic.Result.True;
    }
}