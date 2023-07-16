using System;
using System.Globalization;
using ECS.NET.Utilities;

namespace ECS.NET.Core
{
    // Note: All builder pattern functions return a ref because auto-implemented properties copy and do not retain
    // modifications to the struct
    public unsafe struct Entity : IEquatable<Entity>
    {
        public WorldData* World;
        public Id Id;

        public bool IsAlive => World->EntityIndex.IsAlive(this);

        public Entity(WorldData* world, Id id)
        {
            World = world;
            Id = id;
        }

        public ref Entity Add(ulong id)
        {
            return ref this;
        }

        public void Delete()
        {
            Record* record = World->EntityIndex.TryGet(this);

            if (record != null)
                World->EntityIndex.Delete(this);
        }

        public readonly override string ToString()
        {
            return Id.Value.ToString(CultureInfo.CurrentCulture);
        }

        public readonly override int GetHashCode()
        {
            return (int)Hash.GetHash(Id);
        }

        public readonly bool Equals(Entity other)
        {
            return Id == other.Id;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is Entity entity && Equals(entity);
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        public static implicit operator ulong(Entity entity)
        {
            return entity.Id.Value;
        }

        public static ulong ToUInt64(Entity entity)
        {
            return entity.Id.Value;
        }
    }
}