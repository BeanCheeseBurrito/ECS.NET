using System.Runtime.CompilerServices;
using FluentAssertions;

namespace ECS.NET.Tests
{
    public static class Test
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void True(bool condition)
        {
            condition.Should().BeTrue();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void False(bool condition)
        {
            condition.Should().BeFalse();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Equals<T>(T a, T b)
        {
            a.Should().Be(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Str(string str)
        {
            str.Should().Be(str);
        }
    }
}
