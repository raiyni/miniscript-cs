
namespace Miniscript;

public static class VectorType
{
    static ValMap vectorType;

    public static ValMap Instance()
    {
        if (vectorType != null) return vectorType;

        vectorType = new ValMap();
        vectorType["length"] = Intrinsic.GetByName("length").GetFunc();
        return vectorType;
    }
}