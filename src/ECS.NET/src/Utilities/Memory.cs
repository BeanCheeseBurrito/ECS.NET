using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace ECS.NET.Utilities
{
    /// <summary>
    ///     Static class for managing native memory.
    /// </summary>
    public static unsafe class Memory
    {
        /// <summary>
        ///     A dictionary that maps pointers to allocation sizes
        /// </summary>
        private static readonly ConcurrentDictionary<IntPtr, long> AllocationSizes =
            new ConcurrentDictionary<IntPtr, long>();

        /// <summary>
        ///     Represents the number of non-freed allocations. This is always 0 in release mode.
        /// </summary>
        private static int _aliveAllocations;

        /// <summary>
        ///     Represents the number of non-freed bytes of memory. This is always 0 in release mode.
        /// </summary>
        private static long _aliveBytes;

        public new static string ToString()
        {
            return
                $"Non-freed Allocations: {_aliveAllocations.ToString(CultureInfo.CurrentCulture)}\nNon-freed  bytes: {_aliveBytes.ToString(CultureInfo.CurrentCulture)}";
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void IncrementCounter(IntPtr pointer, long byteCount)
        {
            Interlocked.Increment(ref _aliveAllocations);
            Interlocked.Add(ref _aliveBytes, byteCount);

            if (!AllocationSizes.TryAdd(pointer, byteCount))
                throw new ArgumentException($"Key already exists: {pointer}", nameof(pointer));
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DecrementCounter(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                return;

            if (!AllocationSizes.TryGetValue(pointer, out long allocationSize))
                throw new ArgumentException("Key not found", nameof(pointer));

            Interlocked.Decrement(ref _aliveAllocations);
            Interlocked.Add(ref _aliveBytes, -allocationSize);

            if (!AllocationSizes.TryRemove(pointer, out _))
                throw new ArgumentException("Failed to remove key", nameof(pointer));

            // TODO: Should the user be able to free memory that was allocated outside of this class?
            if (_aliveBytes < 0)
                throw new InvalidOperationException("Freed more memory than was allocated.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Alloc<T>(ulong count) where T : unmanaged
        {
            return (T*)Alloc(count * (ulong)sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* AllocZeroed<T>(ulong count) where T : unmanaged
        {
            return (T*)AllocZeroed(count * (ulong)sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Realloc<T>(T* data, ulong count) where T : unmanaged
        {
            return (T*)Realloc((void*)data, count * (ulong)sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(void* data)
        {
#if NET6_0_OR_GREATER
            NativeMemory.Free(data);
#else
            Marshal.FreeHGlobal((IntPtr)data);
#endif
            DecrementCounter((IntPtr)data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Alloc(ulong byteCount)
        {
#if NET6_0_OR_GREATER
            void* pointer = NativeMemory.Alloc((nuint)byteCount);
#else
            void* pointer = (void*)Marshal.AllocHGlobal((IntPtr)byteCount);
#endif

            if (pointer == null)
                throw new InsufficientMemoryException();

            IncrementCounter((IntPtr)pointer, (long)byteCount);
            return pointer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AllocZeroed(ulong byteCount)
        {
#if NET6_0_OR_GREATER
            void* pointer = NativeMemory.AllocZeroed((nuint)byteCount);
#else
            void* pointer = (void*)Marshal.AllocHGlobal((IntPtr)byteCount);
            Unsafe.InitBlock(pointer, 0, (uint)byteCount);
#endif

            if (pointer == null)
                throw new InsufficientMemoryException();

            IncrementCounter((IntPtr)pointer, (long)byteCount);
            return pointer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Realloc(void* data, ulong byteCount)
        {
#if NET6_0_OR_GREATER
            void* pointer = NativeMemory.Realloc(data, (nuint)byteCount);
#else
            void* pointer = (void*)Marshal.ReAllocHGlobal((IntPtr)data, (IntPtr)byteCount);
#endif

            if (pointer == null)
                throw new InsufficientMemoryException();

#if DEBUG
            DecrementCounter((IntPtr)data);
            IncrementCounter((IntPtr)pointer, (long)byteCount);
#endif

            return pointer;
        }
    }
}
