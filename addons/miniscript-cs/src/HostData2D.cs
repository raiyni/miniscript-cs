using Godot;

namespace Miniscript;

public class HostData2D : HostData
{
    private readonly Node2D self;
    public HostData2D(Node n, ScriptEngine e) : base((Node2D)n, e)
    {
        self = (Node2D)n;
    }

    public override Intrinsic.Result Move(TAC.Context context)
    {
        if (Engine.allowMoving)
        {
            var vec = self.Position;
            vec.X = (float)context.GetVar("x").DoubleValue();
            vec.Y = (float)context.GetVar("y").DoubleValue();

            self.Position = vec;
        }
        
        return Intrinsic.Result.True;
    }
}