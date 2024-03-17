using Godot;

namespace Miniscript;
public partial class ScriptEngine : Node
{
    static bool DisallowAllAssignment(Value key, Value value)
    {
        throw new RuntimeException("Assignment to protected map");
    }
    static ValMap nodeType;

    public static ValMap NodeType()
    {
        if (nodeType != null) return nodeType;

        nodeType = new ValMap();

        Intrinsic f = Intrinsic.Create("");
        f.code = (context, partialResult) =>
        {
            var data = context.interpreter.hostData as HostData;
            return new Intrinsic.Result(new ValString(data.Node.Name));
        };
        nodeType["name"] = f.GetFunc();

        f = Intrinsic.Create("");
        f.AddParam("key");
        f.code = (context, partialResult) =>
        {
            var data = context.interpreter.hostData as HostData;
            var box = (BoxRun)data.Node;
            var key = context.GetLocalString("key");

            if (key == "fireRate") return new Intrinsic.Result(new ValNumber(box.fireRate));

            return Intrinsic.Result.Null;
        };
        nodeType["get"] = f.GetFunc();

        f = Intrinsic.Create("");
        f.AddParam("key");
        f.AddParam("value");
        f.code = (context, partialResult) =>
        {
            var data = context.interpreter.hostData as HostData;
            var box = (BoxRun)data.Node;
            var key = context.GetLocalString("key");

            if (key == "fireRate")
            {
                box.fireRate = context.GetLocalFloat("value");
            }

            return Intrinsic.Result.True;
        };
        nodeType["set"] = f.GetFunc();
        nodeType.assignOverride = DisallowAllAssignment;

        return nodeType;
    }

    static void AddIntrinsics()
    {
        if (intrinsicsAdded) return;
        intrinsicsAdded = true;

        AddMovementIntrinsics();
        AddVector3Intrinsics();
        AddNodeIntrinsics();
    }

    static void AddMovementIntrinsics()
    {
        Intrinsic f = Intrinsic.Create("move");
        f.AddParam("x", 0);
        f.AddParam("y", 0);
        f.AddParam("z", ValNull.instance);
        f.code = (context, partialResult) =>
        {
            var data = context.interpreter.hostData as HostData;
            return data.Move(context);
        };

        f = Intrinsic.Create("getPos");
        f.code = (context, partialResult) =>
        {
            var data = context.interpreter.hostData as HostData;
            return data.GetPos(context);
        };
    }

    static void AddNodeIntrinsics()
    {
        Intrinsic f = Intrinsic.Create("node");
        f.code = (context, partialResult) =>
        {
            HostData sh = context.interpreter.hostData as HostData;
            return new Intrinsic.Result(NodeType());
        };
    }
}