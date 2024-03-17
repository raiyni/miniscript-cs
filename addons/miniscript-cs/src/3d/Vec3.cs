
using Godot;

namespace Miniscript;

public class Vec3 : ValMap
{
    public Vector3 value;

    public Vec3(Vector3 v)
    {
        value = v;
        SetElem(new ValString("x"), new ValNumber(v.X));
        SetElem(new ValString("y"), new ValNumber(v.Y));
        SetElem(new ValString("z"), new ValNumber(v.Z));
    }

    public override double Equality(Value rhs)
    {
        return rhs is Vec3 r && value.Equals(r.value) ? 1 : 0;
    }

    public override int Hash()
    {
        return value.GetHashCode();
    }

    public override string ToString(TAC.Machine vm)
    {
        return value.ToString();
    }

    public override bool IsA(Value type, TAC.Machine vm)
    {
        if (type == null) return false;
        return type == ScriptEngine.Vec3Type();
    }
}