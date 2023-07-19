using System;

namespace ECS.NET.Tests.Collections
{
    public readonly struct TestKey : IEquatable<TestKey>
    {
        public long Field1 { get; }
        public int Field2 { get; }
        public short Field3 { get; }
        public sbyte Field4 { get; }

        public TestKey(long field1 = 0, int field2 = 0, short field3 = 0, sbyte field4 = 0)
        {
            Field1 = field1;
            Field2 = field2;
            Field3 = field3;
            Field4 = field4;
        }

        public bool Equals(TestKey other)
        {
            return other.Field1 == Field1 &&
                   other.Field2 == Field2 &&
                   other.Field3 == Field3 &&
                   other.Field4 == Field4;
        }
    }
}
