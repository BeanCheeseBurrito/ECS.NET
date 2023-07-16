using System;

namespace ECS.NET.Core
{
    public struct WorldData : IDisposable
    {
        public EntityIndex EntityIndex;

        public WorldData(EntityIndex entityIndex)
        {
            EntityIndex = entityIndex;
        }

        public static WorldData Init()
        {
            return new WorldData(EntityIndex.Init());
        }

        public void Dispose()
        {
            EntityIndex.Dispose();
        }
    }
}