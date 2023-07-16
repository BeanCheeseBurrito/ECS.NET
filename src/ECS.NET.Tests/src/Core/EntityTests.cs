using ECS.NET.Core;
using FluentAssertions;
using Xunit;

namespace ECS.NET.Tests.Core;

public class EntityTests
{
    [Fact]
    public void Create()
    {
        using World world = World.Init();

        Entity entity = world.Entity();

        entity.Id.Should().NotBe(default);
    }

    [Fact]
    public void CreateAndDelete()
    {
        using World world = World.Init();

        Entity entity = world.Entity();

        entity.IsAlive.Should().BeTrue();

        entity.Delete();

        entity.IsAlive.Should().BeFalse();
    }
}
