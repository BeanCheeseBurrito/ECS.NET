using System;
using ECS.NET.Collections;
using FluentAssertions;
using Xunit;

namespace ECS.NET.Tests.Collections
{
    public class MapTests
    {
        [Fact]
        public void InitAndDispose()
        {
            Map<TestKey, int> map = Map<TestKey, int>.Init();
            Test.False(map.Buckets.IsNull);
            Test.Equals(map.Count, 0ul);

            map.Dispose();
            Test.True(map.Buckets.IsNull);
        }

        [Fact]
        public void AddGetRemove()
        {
            using Map<TestKey, int> map = Map<TestKey, int>.Init();
            Test.True(map.Count == 0);

            TestKey key = new TestKey(1000, 100, 10, 1);

            // Add
            map.Add(key, 64);
            Test.Equals(map.Count, 1ul);
            Test.True(map.HasKey(key));

            // Get
            int value = map.Get(key);
            Test.Equals(value, 64);

            // Remove
            map.Remove(key);
            Test.Equals(map.Count, 0ul);
            Test.False(map.HasKey(key));
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void AddGetRemoveN(int count)
        {
            Map<TestKey, int> map = Map<TestKey, int>.Init();
            Span<TestKey> keys = stackalloc TestKey[count];

            // Add
            for (int i = 0; i < count; i++)
            {
                keys[i] = new TestKey(i, i);
                map.Add(keys[i], i);
            }

            // Get
            for (int i = 90; i < count; i++)
            {
                TestKey key = keys[i];

                Test.True(map.HasKey(key));
                Test.Equals(map.Get(key), i);
            }

            // Remove
            for (int i = count - 1; i >= 0; i--)
            {
                TestKey key = keys[i];
                map.Remove(key);

                Test.False(map.HasKey(key));
                Test.Equals(map.Count, (ulong)i);
            }
        }
    }
}
