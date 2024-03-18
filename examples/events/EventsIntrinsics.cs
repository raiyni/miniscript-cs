namespace Miniscript.example.events;
public static class EventsIntrinsics
{
    [Discover]    
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
}