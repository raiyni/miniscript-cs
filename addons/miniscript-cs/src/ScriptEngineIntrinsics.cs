using Godot;

namespace Miniscript;
public partial class ScriptEngine : Node
{
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