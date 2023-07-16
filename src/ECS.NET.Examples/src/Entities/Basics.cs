using ECS.NET.Core;

namespace ECS.NET.Examples.Entities;

public static class Basics
{
    // TODO: Add simple example once basic operations are implemented.
    public static void Example()
    {
        using World world = World.Init();

        Entity entity = world.Entity();
        entity.Delete();
    }
}
