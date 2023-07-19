using System;
using ECS.NET.Utilities;

namespace ECS.NET.Collections
{
    public unsafe struct Arr<T> : IDisposable where T : unmanaged
    {
        public T* Data;
        public ulong Size;

        public readonly bool IsNull => Data == null;
        public readonly Span<T> Span => new Span<T>(Data, (int)Size);

        public static Arr<T> Init(ulong size = 0)
        {
            return new Arr<T>()
            {
                Size = size,
                Data = size == 0 ? null : Memory.Alloc<T>(size)
            };
        }

        public static Arr<T> InitZeroed(ulong size = 0)
        {
            return new Arr<T>()
            {
                Size = size,
                Data = size == 0 ? null : Memory.AllocZeroed<T>(size)
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

        public T* PointerAt(ulong index)
        {
            return &Data[index];
        }
    }
}
