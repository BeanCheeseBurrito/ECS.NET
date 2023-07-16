using System;
using System.Diagnostics.CodeAnalysis;

namespace ECS.NET.Utilities
{
    /// <summary>
    ///     Thin wrapper around pointer for use as a generic type argument.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressMessage("Usage", "CA1720")]
    public unsafe struct Ptr<T> : IEquatable<Ptr<T>> where T : unmanaged
    {
        public T* Pointer;

        public readonly bool IsNull => Pointer == null;

        public Ptr(T* pointer)
        {
            Pointer = pointer;
        }

        public readonly override int GetHashCode()
        {
            return (int)Hash.GetHash(this);
        }

        public readonly bool Equals(Ptr<T> other)
        {
            return Pointer == other.Pointer;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is IntPtr ptr && (IntPtr)Pointer == ptr;
        }

        public static bool operator ==(Ptr<T> left, Ptr<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ptr<T> left, Ptr<T> right)
        {
            return !(left == right);
        }

        public static implicit operator T*(Ptr<T> pointer)
        {
            return pointer.Pointer;
        }

        public static implicit operator Ptr<T>(T* pointer)
        {
            return new Ptr<T>(pointer);
        }

        public static T* To(Ptr<T> pointer)
        {
            return pointer.Pointer;
        }

        public static Ptr<T> From(T* pointer)
        {
            return new Ptr<T>(pointer);
        }
    }
}