using Miniscript.example.vector;

namespace Miniscript.example.events;
public static class EventsIntrinsics
{
    [Discover]
    static void AddMovementIntrinsics()
    {
        Intrinsic f = Intrinsic.Create("move");
        f.AddParam("x", 0);
        f.AddParam("y", 0);
        f.AddParam("z", ValNull.instance);
        f.code = (context, partialResult) =>
        {
            var data = context.interpreter.hostData as HostData;
            var engine = (S2)data.Engine;
            if (engine.allowMoving)
            {
                var vec = data.Node3D.Position;
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


                data.Node3D.Position = vec;
            }

            return Intrinsic.Result.True;
        };

        f = Intrinsic.Create("getPos");
        f.code = (context, partialResult) =>
        {
            var data = context.interpreter.hostData as HostData;
            return new Intrinsic.Result(new Vec3(data.Node3D.Position));
        };
    }
}