using System.Runtime.CompilerServices;

namespace ECS.NET.Core
{
    public class Ecs
    {
        public const ulong Pair = 1ul << 63;

        public const ulong GenerationMask = 0xFFFFul << 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Generation(ulong entity)
        {
            return (entity & GenerationMask) >> 32;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong IncrementGeneration(ulong entity)
        {
            return (entity & ~GenerationMask) | ((0xFFFF & (Generation(entity) + 1)) << 32);
        }
    }
}