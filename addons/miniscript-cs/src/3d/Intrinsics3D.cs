using Godot;

namespace Miniscript;

public partial class ScriptEngine : Node
{
    static ValMap vec3Type;

    public static ValMap Vec3Type()
    {
        if (vec3Type != null) return vec3Type;

        vec3Type = new ValMap();
        vec3Type["test"] = Intrinsic.GetByName("test").GetFunc();
        return vec3Type;
    }

    static void AddVector3Intrinsics()
    {
        Intrinsic f = Intrinsic.Create("test");
        f.AddParam("self");
        f.AddParam("idx");
        f.code = (context, partialResult) => 
        {
			Value self = context.self;
            Value idx = context.GetLocal("idx"); 

            GD.Print(self?.GetType(), idx);
            return Intrinsic.Result.True;
        };

        f = Intrinsic.Create("vec3");
        f.code = (context, partialResult) =>
        {
            if (vec3Type == null)
            {
                vec3Type = Vec3Type().EvalCopy(context.vm.globalContext);
            }
            return new Intrinsic.Result(vec3Type);
        };      
    }
}