using System;
using System.Runtime.CompilerServices;
using ECS.NET.Utilities;

namespace ECS.NET.Collections
{
    public unsafe struct Vec<T> : IDisposable where T : unmanaged
    {
        public T* Data;
        public ulong Capacity;
        public ulong Count;

        public readonly Span<T> Span => new Span<T>(Data, (int)Count);

        private const uint DefaultCapacity = 4;

        public static Vec<T> Init(ulong capacity = 0)
        {
            if (capacity == 0)
                capacity = DefaultCapacity;

            return new Vec<T>
            {
                Data = Memory.Alloc<T>(capacity),
                Capacity = capacity,
                Count = 0
            };
        }

        public static Vec<T> InitZero(ulong capacity = 0)
        {
            if (capacity == 0)
                capacity = DefaultCapacity;

            return new Vec<T>
            {
                Data = Memory.AllocZeroed<T>(capacity),
                Capacity = capacity,
                Count = 0
            };
        }

        public readonly T this[long i]
        {
            get => Data[i];
            set => Data[i] = value;
        }

        public readonly T this[ulong i]
        {
            get => Data[i];
            set => Data[i] = value;
        }

        public void Dispose()
        {
            if (Data == null)
                return;

            Memory.Free(Data);

            Data = null;
        }

        public void Add(T item)
        {
            if (Count >= Capacity)
                EnsureCapacity(Capacity * 2);

            Data[Count++] = item;
        }

        public readonly T* PointerAt(ulong index)
        {
            return &Data[index];
        }

        public void EnsureCapacity(ulong newCapacity, bool initZero = false)
        {
            if (newCapacity <= Capacity)
                return;

            T* ptr = Memory.Realloc(Data, newCapacity);

            if (initZero)
                Unsafe.InitBlock(&ptr[Count], 0, (uint)((newCapacity - Count) * (uint)sizeof(T)));

            Data = ptr;
            Capacity = newCapacity;
        }

        public void SetMinCount(ulong minCount, bool initZero = false)
        {
            if (Count > minCount)
                return;

            EnsureCapacity(minCount, initZero);
            Count = minCount;
        }
    }
}