using System;
using ECS.NET.Utilities;

namespace ECS.NET.Core
{
    public struct Table : IEquatable<Table>
    {
        public readonly override int GetHashCode()
        {
            return (int)Hash.GetHash(this);
        }

        public readonly bool Equals(Table table)
        {
            return true;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is Table table && Equals(table);
        }

        public static bool operator ==(Table left, Table right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Table left, Table right)
        {
            return !(left == right);
        }
    }
}