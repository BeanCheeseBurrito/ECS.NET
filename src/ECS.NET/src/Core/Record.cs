using System;
using ECS.NET.Utilities;

namespace ECS.NET.Core
{
    public struct Record : IEquatable<Record>
    {
        public Table Table;
        public ulong Row;
        public ulong Dense;

        public readonly override int GetHashCode()
        {
            return (int)Hash.GetHash(this);
        }

        public readonly bool Equals(Record other)
        {
            return Table == other.Table && Row == other.Row && Dense == other.Dense;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is Record record && Equals(record);
        }

        public static bool operator ==(Record left, Record right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Record left, Record right)
        {
            return !(left == right);
        }
    }
}