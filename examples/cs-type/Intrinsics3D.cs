using Godot;

namespace Miniscript.example.vector;

public static class Intrinsics3D
{
    [Discover]
    static void AddVector3Intrinsics()
    {
        Intrinsic f = Intrinsic.Create("length");
        f.AddParam("v");
        f.code = (context, partialResult) => 
        {
			Value v = context.GetLocal("v");
            if (v is Vec3 vec3)
            {
                return new Intrinsic.Result(new ValNumber(vec3.Length()));
            }

            return Intrinsic.Result.Null;
        };

        f = Intrinsic.Create("vec3");
        f.AddParam("x", 0);
        f.AddParam("y", 0);
        f.AddParam("z", 0);
        f.code = (context, partialResult) =>
        {
            var x = context.GetLocal("x");
            var y = context.GetLocal("y"); 
            var z = context.GetLocal("z");
            return new Intrinsic.Result(new Vec3(new Vector3(x.FloatValue(), y.FloatValue(), z.FloatValue())));
        };      
    }
}