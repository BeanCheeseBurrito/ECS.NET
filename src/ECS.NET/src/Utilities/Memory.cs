using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        // TODO: Replace with unmanaged hashmap once it is implemented?
        private static readonly ConcurrentDictionary<UIntPtr, ulong> AllocationSizes = new ConcurrentDictionary<UIntPtr, ulong>();

        /// <summary>
        ///     Represents the number of non-freed allocations. This is always 0 in release mode.
        /// </summary>
        public static int AliveAllocations { get; private set; } // TODO: Change to unsigned integer?

        /// <summary>
        ///     Represents the number of non-freed bytes of memory. This is always 0 in release mode.
        /// </summary>
        public static long AliveBytes { get; private set; } // TODO: Change to unsigned integer?

        public new static string ToString()
        {
            return
                $"Non-freed Allocations: {AliveAllocations.ToString(CultureInfo.CurrentCulture)}\nNon-freed  bytes: {AliveBytes.ToString(CultureInfo.CurrentCulture)}";
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void IncrementCounter(void* pointer, ulong byteCount)
        {
            AliveAllocations++;
            AliveBytes += (long)byteCount;

            if (!AllocationSizes.TryAdd((UIntPtr)pointer, byteCount))
                throw new ArgumentException($"Key already exists: {((IntPtr)pointer).ToString()}", nameof(pointer));
        }

        [Conditional("DEBUG")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DecrementCounter(void* pointer)
        {
            if (pointer == null)
                return;

            AliveAllocations--;
            AliveBytes -= (long)AllocationSizes[(UIntPtr)pointer];

            if (!AllocationSizes.TryRemove((UIntPtr)pointer, out _))
                throw new ArgumentException("Failed to remove key", nameof(pointer));

            // TODO: Should the user be able to free memory that was allocated outside of this class?
            if (AliveBytes < 0)
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
            DecrementCounter(data);
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

            IncrementCounter(pointer, byteCount);
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

            IncrementCounter(pointer, byteCount);
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
            DecrementCounter(data);
            IncrementCounter(pointer, byteCount);
#endif

            return pointer;
        }
    }
}
