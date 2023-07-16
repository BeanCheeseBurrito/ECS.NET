using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ECS.NET.Utilities
{
    public static unsafe class Hash
    {
        public static readonly ulong* DefaultSecrets = (ulong*)Marshal.AllocHGlobal(sizeof(ulong) * 4);

        static Hash()
        {
            DefaultSecrets[0] = 0xa0761d6478bd642ful;
            DefaultSecrets[1] = 0xe7037ed1a0b428dbul;
            DefaultSecrets[2] = 0x8ebc6af09c88c6e3ul;
            DefaultSecrets[3] = 0x589965cc75374cc3ul;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetHash<T>(T span) where T : unmanaged
        {
            return WyHash(&span, (ulong)sizeof(T), 0, DefaultSecrets);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetHash<T>(ReadOnlySpan<T> span) where T : unmanaged
        {
            fixed (void* key = span)
            {
                return WyHash(key, (ulong)(sizeof(T) * span.Length), 0, DefaultSecrets);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetHash(string str)
        {
            return GetHash(str.AsSpan());
        }

        // https://github.com/wangyi-fudan/wyhash/blob/master/wyhash.h
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong WyHash(void* key, ulong len, ulong seed, ulong* secret)
        {
            byte* p = (byte*)key;
            seed ^= WyMix(seed ^ secret[0], secret[1]);

            ulong a;
            ulong b;

            if (len <= 16)
            {
                if (len >= 4)
                {
                    a = (WyR4(p) << 32) | WyR4(p + ((len >> 3) << 2));
                    b = (WyR4(p + len - 4) << 32) | WyR4(p + len - 4 - ((len >> 3) << 2));
                }
                else if (len > 0)
                {
                    a = WyR3(p, len);
                    b = 0;
                }
                else
                {
                    a = 0;
                    b = 0;
                }
            }
            else
            {
                ulong i = len;

                if (i > 48)
                {
                    ulong see1 = seed;
                    ulong see2 = seed;

                    do
                    {
                        seed = WyMix(WyR8(p) ^ secret[1], WyR8(p + 8) ^ seed);
                        see1 = WyMix(WyR8(p + 16) ^ secret[2], WyR8(p + 24) ^ see1);
                        see2 = WyMix(WyR8(p + 32) ^ secret[3], WyR8(p + 40) ^ see2);
                        p += 48;
                        i -= 48;
                    } while (i > 48);

                    seed ^= see1 ^ see2;
                }

                while (i > 16)
                {
                    seed = WyMix(WyR8(p) ^ secret[1], WyR8(p + 8) ^ seed);
                    i -= 16;
                    p += 16;
                }

                a = WyR8(p + i - 16);
                b = WyR8(p + i - 8);
            }

            a ^= secret[1];
            b ^= seed;
            WyMum(&a, &b);

            return WyMix(a ^ secret[0] ^ len, b ^ secret[1]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WyMum(ulong* a, ulong* b)
        {
            ulong ha = *a >> 32;
            ulong hb = *b >> 32;
            ulong la = (uint)*a;
            ulong lb = (uint)*b;

            ulong rh = ha * hb;
            ulong rm0 = ha * lb;
            ulong rm1 = hb * la;
            ulong rl = la * lb;

            ulong t = rl + (rm0 << 32);

            ulong lo = t + (rm1 << 32);
            ulong hi = rh + (rm0 >> 32) + (rm1 >> 32) + (t < rl ? 1ul : 0ul) + (lo < t ? 1ul : 0ul);

            *a ^= lo;
            *b ^= hi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong WyMix(ulong a, ulong b)
        {
            WyMum(&a, &b);
            return a ^ b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong WyR8(byte* p)
        {
            ulong v = *(ulong*)p;
            return ((v >> 56) & 0xff) | ((v >> 40) & 0xff00) | ((v >> 24) & 0xff0000) | ((v >> 8) & 0xff000000) |
                   ((v << 8) & 0xff00000000) | ((v << 24) & 0xff0000000000) | ((v << 40) & 0xff000000000000) |
                   ((v << 56) & 0xff00000000000000);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong WyR4(byte* p)
        {
            uint v = *(uint*)p;
            return ((v >> 24) & 0xff) | ((v >> 8) & 0xff00) | ((v << 8) & 0xff0000) | ((v << 24) & 0xff000000);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong WyR3(byte* p, ulong k)
        {
            return ((ulong)p[0] << 16) | ((ulong)p[k >> 1] << 8) | p[k - 1];
        }
    }
}