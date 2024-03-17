using Godot;

namespace Miniscript;

public class HostData3D : HostData
{
    private readonly Node3D self;
    public HostData3D(Node n, ScriptEngine e) : base((Node3D)n, e)
    {
        self = (Node3D)n;
    }

    public override Intrinsic.Result Move(TAC.Context context)
    {
        if (Engine.allowMoving)
        {
            var vec = self.Position;
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


            self.Position = vec;
        }

        return Intrinsic.Result.True;
    }

    public override Intrinsic.Result GetPos(TAC.Context context)
    {
        return new Intrinsic.Result(new Vec3(self.Position));
    }
}