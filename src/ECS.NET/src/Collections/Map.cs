using System;
using ECS.NET.Utilities;

namespace ECS.NET.Collections
{
    public unsafe struct Map<TKey, TValue> : IDisposable
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged
    {
        public Arr<Ptr<Bucket>> Buckets;
        public ulong Count;

        public const float LoadFactor = 0.70f;

        public static Map<TKey, TValue> Init()
        {
            return new Map<TKey, TValue>()
            {
                Buckets = Arr<Ptr<Bucket>>.InitZeroed(4)
            };
        }

        public TValue this[TKey key]
        {
            get => Get(key);
        }

        public void Dispose()
        {
            if (Buckets.IsNull)
                return;

            for (ulong i = 0; i < Buckets.Size; i++)
            {
                Bucket* bucket = Buckets[i];

                while (bucket != null)
                {
                    Bucket* prevBucket = bucket;
                    bucket = bucket->Next;
                    Memory.Free(prevBucket);
                }
            }

            Buckets.Dispose();
        }

        public void Add(TKey key, TValue value)
        {
            if ((float)Count / Buckets.Size >= LoadFactor)
                Resize(Buckets.Size * 2);

            ulong hash = Hash.GetHash(key);
            ulong bucketIndex = hash % Buckets.Size;

            AddToBucket((Bucket**)Buckets.PointerAt(bucketIndex), hash, key, value);
            Count++;
        }

        private void AddToBucket(Bucket** bucketPtr, ulong hash, TKey key, TValue value)
        {
            Bucket* bucket = *bucketPtr;
            Bucket* prevBucket = null;

            while (bucket != null)
            {
                if (bucket->Key.Equals(key))
                    throw new ArgumentException("Key already exists", nameof(key));

                prevBucket = bucket;
                bucket = bucket->Next;
            }

            bucket = Memory.AllocZeroed<Bucket>(1);
            bucket->Hash = hash;
            bucket->Key = key;
            bucket->Value = value;

            if (prevBucket == null)
            {
                *bucketPtr = bucket;
                return;
            }

            prevBucket->Next = bucket;
        }

        public void Remove(TKey key)
        {
            ulong hash = Hash.GetHash(key);
            ulong bucketIndex = hash % Buckets.Size;

            Bucket* bucket = Buckets[bucketIndex];
            Bucket* prevBucket = null;

            while (bucket != null)
            {
                if (bucket->Key.Equals(key))
                {
                    if (prevBucket != null)
                        prevBucket->Next = bucket->Next;
                    else
                        Buckets[bucketIndex] = null;

                    Memory.Free(bucket);
                    Count--;
                    return;
                }

                prevBucket = bucket;
                bucket = bucket->Next;
            }
        }

        public readonly bool HasKey(TKey key)
        {
            ulong hash = Hash.GetHash(key);
            ulong bucketIndex = hash % Buckets.Size;

            Bucket* bucket = Buckets[bucketIndex];

            while (bucket != null)
            {
                if (bucket->Key.Equals(key))
                    return true;

                bucket = bucket->Next;
            }

            return false;
        }

        public readonly TValue Get(TKey key)
        {
            ulong hash = Hash.GetHash(key);
            ulong bucketIndex = hash % Buckets.Size;

            Bucket* bucket = Buckets[bucketIndex];

            while (bucket != null)
            {
                if (bucket->Key.Equals(key))
                    return bucket->Value;

                bucket = bucket->Next;
            }

            throw new ArgumentException("Key doesn't exist", nameof(key));
        }

        public void Resize(ulong newSize)
        {
            Arr<Ptr<Bucket>> newBuckets = Arr<Ptr<Bucket>>.InitZeroed(newSize);

            for (ulong i = 0; i < Buckets.Size; i++)
            {
                Bucket* bucket = Buckets[i];

                while (bucket != null)
                {
                    ulong hash = bucket->Hash;
                    ulong bucketIndex = hash % newSize;

                    AddToBucket((Bucket**)newBuckets.PointerAt(bucketIndex), hash, bucket->Key, bucket->Value);

                    Bucket* prevBucket = bucket;
                    bucket = bucket->Next;

                    Memory.Free(prevBucket);
                }
            }

            Buckets.Dispose();
            Buckets = newBuckets;

            Console.WriteLine(newSize);
        }

        public struct Bucket
        {
            public ulong Hash;
            public Bucket* Next;
            public TKey Key;
            public TValue Value;
        }
    }
}
