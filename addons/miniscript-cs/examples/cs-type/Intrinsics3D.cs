using Godot;

namespace Miniscript;

public partial class ScriptEngine : Node
{
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
        f.code = (context, partialResult) =>
        {
            return new Intrinsic.Result(new Vec3(Vector3.Zero));
        };      
    }
}