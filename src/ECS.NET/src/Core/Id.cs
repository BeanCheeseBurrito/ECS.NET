using System;
using ECS.NET.Utilities;

namespace ECS.NET.Core
{
    /// <summary>
    ///     An Id is a unsigned 64-bit integer that can represent an entity, an entity with flags, or a pair of two entities
    /// </summary>
    public readonly struct Id : IEquatable<Id>
    {
        public readonly ulong Value;

        public bool IsPair => (Value & Ecs.Pair) == 1;

        public Id(ulong id)
        {
            Value = id;
        }

        public override int GetHashCode()
        {
            return (int)Hash.GetHash(Value);
        }

        public bool Equals(Id other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Id id && Equals(id);
        }

        public static bool operator ==(Id left, Id right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Id left, Id right)
        {
            return !(left == right);
        }

        public static implicit operator ulong(Id id)
        {
            return id.Value;
        }

        public static implicit operator Id(ulong id)
        {
            return new Id(id);
        }

        public static ulong ToUInt64(Id id)
        {
            return id.Value;
        }

        public static Id FromUInt64(ulong id)
        {
            return new Id(id);
        }
    }
}