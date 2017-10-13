using System;
using System.Runtime.CompilerServices;

namespace Isaac
{
    public class Isaac64
    {
        //-Sourced from Bob Jenkins http://www.burtleburtle.net/bob/rand/isaacafa.html
        //(c) Bob Jenkins, March 1996, Public Domain
        //You may use this code in any way you wish, and it is free.  No warrantee.
        //http://burtleburtle.net/bob/rand/isaacafa.html
        //-Modified by Dustin Sparks (c) 2017, Public Domain

        private const UInt64 CONST_PHI = 0x9E3779B97F4A7C13UL;
        private const ushort CONST_STATE_LEN = 256;
        private const ushort CONST_STATE_LEN_HALF = CONST_STATE_LEN / 2;
        internal UInt64[] state = new UInt64[CONST_STATE_LEN];
        public UInt64[] result = new UInt64[CONST_STATE_LEN];
        internal ushort state_idx = 0;
        internal UInt64 aa, bb, cc;

        public Isaac64()
        {
            // Zero internals
            for (int n = 0; n < CONST_STATE_LEN; ++n)
            {
                result[n] = state[n] = 0;
            }
            Seed(false);
        }

        public Isaac64(UInt64[] seed)
        {
            // Zero result block, folding in seed
            for (int n = 0; n < CONST_STATE_LEN; ++n)
            {
                state[n] = 0;
                if (seed != null && n < seed.Length)
                    result[n] = seed[n];
                else
                    result[n] = 0;
            }
            Seed(seed != null && seed.Length > 0);
        }

        public Isaac64(bool Random)
        {
            if (Random)
            {
                for (int n = 0; n < CONST_STATE_LEN; ++n)
                {
                    state[n] = 0;
                }
                System.Security.Cryptography.RNGCryptoServiceProvider dotnetRNG = new System.Security.Cryptography.RNGCryptoServiceProvider();
                byte[] randomSeed = new byte[CONST_STATE_LEN * sizeof(UInt64)];
                dotnetRNG.GetBytes(randomSeed);
                Buffer.BlockCopy(randomSeed, 0, result, 0, randomSeed.Length);
                Seed(true);
            }
            else
                Seed(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MIX(ref UInt64 a, ref UInt64 b, ref UInt64 c, ref UInt64 d, ref UInt64 e, ref UInt64 f, ref UInt64 g, ref UInt64 h)
        {
            a -= e; f ^= h >> 9; h += a;
            b -= f; g ^= a << 9; a += b;
            c -= g; h ^= b >> 23; b += c;
            d -= h; a ^= c << 15; c += d;
            e -= a; b ^= d >> 14; d += e;
            f -= b; c ^= e << 20; e += f;
            g -= c; d ^= f >> 17; f += g;
            h -= d; e ^= g << 14; g += h;
        }

        private void Seed(bool flag = false)
        {
            UInt64 a, b, c, d, e, f, g, h;
            aa = bb = cc = 0;
            a = b = c = d = e = f = g = h = CONST_PHI;

            // Scramble
            for (int n = 0; n < 4; ++n)
            {
                MIX(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h);
            }

            for (int n = 0; n < CONST_STATE_LEN; n += 8)
            {
                // Populated with seed
                if (flag == true)
                {
                    a += result[n];
                    b += result[n + 1];
                    c += result[n + 2];
                    d += result[n + 3];
                    e += result[n + 4];
                    f += result[n + 5];
                    g += result[n + 6];
                    h += result[n + 7];
                }

                MIX(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h);

                // Initialize state
                state[n] = a;
                state[n + 1] = b;
                state[n + 2] = c;
                state[n + 3] = d;
                state[n + 4] = e;
                state[n + 5] = f;
                state[n + 6] = g;
                state[n + 7] = h;
            }

            // Second pass
            if (flag == true)
            {
                for (int n = 0; n < CONST_STATE_LEN; n += 8)
                {
                    a += state[n];
                    b += state[n + 1];
                    c += state[n + 2];
                    d += state[n + 3];
                    e += state[n + 4];
                    f += state[n + 5];
                    g += state[n + 6];
                    h += state[n + 7];

                    MIX(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h);

                    state[n] = a;
                    state[n + 1] = b;
                    state[n + 2] = c;
                    state[n + 3] = d;
                    state[n + 4] = e;
                    state[n + 5] = f;
                    state[n + 6] = g;
                    state[n + 7] = h;
                }
            }

            Shuffle();
            state_idx = CONST_STATE_LEN;
        }

        public void Shuffle()
        {
            UInt64 x, y;
            bb += ++cc;

            for (int n = 0; n < CONST_STATE_LEN; ++n)
            {
                x = state[n];
                switch (n % 4)
                {
                    case 0: aa = ~(aa ^ (aa << 21)); break;
                    case 1: aa ^= (aa >> 5); break;
                    case 2: aa ^= (aa << 12); break;
                    case 3: aa ^= (aa >> 33); break;
                }
                aa += state[(n + CONST_STATE_LEN_HALF) % CONST_STATE_LEN];
                state[n] = y = state[(x >> 3) % CONST_STATE_LEN] + aa + bb;
                result[n] = bb = state[(y >> 11) % CONST_STATE_LEN] + x;
            }
        }

        public UInt64 Next()
        {
            if (state_idx-- == 0)
            {
                Shuffle();
                state_idx = CONST_STATE_LEN - 1;
            }
            return result[state_idx];
        }

        public UInt64[] Next(short bCount)
        {
            UInt64[] localResults = new UInt64[bCount];
            for (int i = 0; i < localResults.Length; i++)
            {
                localResults[i] = Next();
            }
            return localResults;
        }

        public void Next(ref UInt64[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            Next(ref array, 0, array.Length);
        }

        public void Next(ref UInt64[] array, int index, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (index < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException("", "Index and Count must be positive integers.");
            }
            if (index + count > array.Length)
            {
                throw new ArgumentOutOfRangeException("index", "The parameters provided will go past the end of the array. Out of bounds error.");
            }
            for (int i = 0; i < count; i++)
            {
                array[i + index] = Next();
            }
        }
    }
}
