using Godot;
using Miniscript;
using System;

namespace Miniscript.example.access;

public partial class AddIntrinsics : Node
{

    static ValMap nodeType;

    static bool DisallowAllAssignment(Value key, Value value)
    {
        throw new RuntimeException("Assignment to protected map");
    }

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
            var box = (FireableBox)data.Node;
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
            var box = (FireableBox)data.Node;
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
    
    [Discover]
    public static void AddNodeIntrinsics()
    {
        if (nodeType != null) return;
        NodeType();
        
        Intrinsic f = Intrinsic.Create("node");
        f.code = (context, partialResult) =>
        {
            HostData sh = context.interpreter.hostData as HostData;
            return new Intrinsic.Result(NodeType());
        };
    }
}
