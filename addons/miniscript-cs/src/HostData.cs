using Godot;

namespace Miniscript;

public class HostData
{
    public Node Node { get; }
    public ScriptEngine Engine { get; }

    public HostData(Node n, ScriptEngine e)
    {
        Node = n;
        Engine = e;
    }

    public virtual Intrinsic.Result Move(TAC.Context context)
    {
        return Intrinsic.Result.True;
    }

    public virtual Intrinsic.Result GetPos(TAC.Context context)
    {
        return Intrinsic.Result.True;
    }
}