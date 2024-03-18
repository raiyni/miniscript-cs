
using Godot;

namespace Miniscript.example.vector;

public class Vec3 : ValMap
{
    public Vector3 value;

    public Vec3(Vector3 v) : base()
    {
        value = v;
        this["x"] = new ValNumber(v.X);
        this["y"] = new ValNumber(v.Y);
        this["z"] = new ValNumber(v.Z);
    }

    public override void SetElem(Value index, Value value)
    {
        index ??= ValNull.instance;
        if (value is ValNumber)
        {
            var key = index.ToString();
            var v = value.FloatValue();

            switch(key)
            {
                case "x":
                    this.value.X = v;
                    break;
                case "y":
                    this.value.Y = v;
                    break;
                case "z":
                    this.value.Z = v;
                    break;
            }
        }
    }

    public float Length()
    {
        return value.Length();
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
        return type.GetType() == GetType();
    }
}