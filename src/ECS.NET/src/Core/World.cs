using System;
using ECS.NET.Utilities;

namespace ECS.NET.Core
{
    /// <summary>
    ///     A <see cref="World" /> represents a collection of entities and components.
    /// </summary>
    public unsafe struct World : IDisposable
    {
        public WorldData* Data;

        public World(WorldData* worldData)
        {
            Data = worldData;
        }

        public static World Init()
        {
            WorldData* pointer = Memory.Alloc<WorldData>(1);
            *pointer = WorldData.Init();
            return new World(pointer);
        }

        public void Dispose()
        {
            Delete();
        }

        public void Delete()
        {
            if (Data == null)
                return;

            Data->Dispose();
            Memory.Free(Data);
            Data = null;
        }

        public Entity Entity()
        {
            return new Entity(Data, Data->EntityIndex.NewId());
        }
    }
}