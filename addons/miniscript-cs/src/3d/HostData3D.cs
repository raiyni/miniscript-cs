using Godot;

namespace Miniscript;

public class HostData3D : HostData
{
    new public Node3D Node { get; }
    new public ScriptEngine Engine { get; }

    public HostData3D(Node n, ScriptEngine e)
    {
        Node = (Node3D)n;
        Engine = e;
    }

    public override Intrinsic.Result Move(TAC.Context context)
    {
        if (Engine.allowMoving)
        {
            var vec = Node.Position;
            vec.X = (float)context.GetVar("x").DoubleValue();

            var z = context.GetVar("z");
            if (z is ValNull)
            {
                vec.Z = (float)context.GetVar("y").DoubleValue();
            }
            else
            {
                vec.Y = (float)context.GetVar("y").DoubleValue();
                vec.Z = (float)context.GetVar("z").DoubleValue();
            }


            Node.Position = vec;
        }

        return Intrinsic.Result.True;
    }

    public override Intrinsic.Result GetPos(TAC.Context context)
    {
        return new Intrinsic.Result(new Vec3(Node.Position));
    }
}