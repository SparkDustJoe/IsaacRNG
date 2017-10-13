using System;
using System.Runtime.CompilerServices;

namespace Isaac
{
    public class Isaac32
    {
        //-Sourced from Bob Jenkins http://www.burtleburtle.net/bob/rand/isaacafa.html
        //(c) Bob Jenkins, March 1996, Public Domain
        //You may use this code in any way you wish, and it is free.  No warrantee.
        //http://burtleburtle.net/bob/rand/isaacafa.html
        //-Modified by Dustin Sparks (c) 2017, Public Domain

        private const UInt32 CONST_PHI = 0x9E3779B9U;
        private const ushort CONST_STATE_LEN = 256;
        private const ushort CONST_STATE_LEN_HALF = CONST_STATE_LEN / 2;
        internal UInt32[] state = new UInt32[CONST_STATE_LEN];
        public UInt32[] result = new UInt32[CONST_STATE_LEN];
        internal ushort state_idx = 0;
        internal UInt32 aa, bb, cc;

        public Isaac32()
        {
            // Zero internals 
            for (int n = 0; n < CONST_STATE_LEN; ++n)
            {
                result[n] = state[n] = 0;
            }
            Seed(false);
        }

        public Isaac32(UInt32[] seed)
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

        public Isaac32(bool Random)
        {
            if (Random)
            {
                for (int n = 0; n < CONST_STATE_LEN; ++n)
                {
                    state[n] = 0;
                }
                System.Security.Cryptography.RNGCryptoServiceProvider dotnetRNG = new System.Security.Cryptography.RNGCryptoServiceProvider();
                byte[] randomSeed = new byte[CONST_STATE_LEN * sizeof(UInt32)];
                dotnetRNG.GetBytes(randomSeed);
                Buffer.BlockCopy(randomSeed, 0, result, 0, randomSeed.Length);
                Seed(true);
            }
            else
                Seed(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MIX(ref UInt32 a, ref UInt32 b, ref UInt32 c, ref UInt32 d, ref UInt32 e, ref UInt32 f, ref UInt32 g, ref UInt32 h)
        {
            a ^= b << 11; d += a; b += c;
            b ^= c >> 2; e += b; c += d;
            c ^= d << 8; f += c; d += e;
            d ^= e >> 16; g += d; e += f;
            e ^= f << 10; h += e; f += g;
            f ^= g >> 4; a += f; g += h;
            g ^= h << 8; b += g; h += a;
            h ^= a >> 9; c += h; a += b;
        }

        private void Seed(bool flag = false)
        {
            UInt32 a, b, c, d, e, f, g, h;
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
            UInt32 x, y;
            bb += ++cc;

            for (int n = 0; n < CONST_STATE_LEN; ++n)
            {
                x = state[n];
                switch (n % 4)
                {
                    case 0: aa ^= (aa << 13); break;
                    case 1: aa ^= (aa >> 6); break;
                    case 2: aa ^= (aa << 2); break;
                    case 3: aa ^= (aa >> 16); break;
                }
                aa += state[(n + CONST_STATE_LEN_HALF) % CONST_STATE_LEN];
                state[n] = y = state[(x >> 2) % CONST_STATE_LEN] + aa + bb;
                result[n] = bb = state[(y >> 10) % CONST_STATE_LEN] + x;
            }
        }

        public UInt32 Next()
        {
            if (state_idx-- == 0)
            {
                Shuffle();
                state_idx = CONST_STATE_LEN - 1;
            }
            return result[state_idx];
        }

        public UInt32[] Next(short bCount)
        {
            UInt32[] localResults = new UInt32[bCount];
            for (int i = 0; i < localResults.Length; i++)
            {
                localResults[i] = Next();
            }
            return localResults;
        }

        public void Next(ref UInt32[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            Next(ref array, 0, array.Length);
        }

        public void Next(ref UInt32[] array, int index, int count)
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


