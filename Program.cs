using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isaac;
using System.Text.RegularExpressions;

namespace Isaac.Tester
{
    class Program
    {
        static string VerifyMeVectors = null;
        static string VerifyMeSeed = null;
        static string VerifyMeVectors64 = null;
        static string VerifyMeSeed64 = null;
        static void Main(string[] args)
        {
            VerifyMeVectors = Regex.Replace(Isaac.Properties.Resources.randvect, "\\s+", "");
            VerifyMeSeed = Regex.Replace(Isaac.Properties.Resources.randseed, "\\s+", "");
            VerifyMeVectors64 = Regex.Replace(Isaac.Properties.Resources.randv64, "\\s+", "");
            VerifyMeSeed64 = Regex.Replace(Isaac.Properties.Resources.rands64, "\\s+", "");

            Console.WriteLine("VECTOR TEST = " + RunVectorTest32(false).ToString());
            Console.WriteLine("SEED TEST = " + RunSeedTest32(false).ToString());
            Console.WriteLine("VECTOR 64 TEST = " + RunVectorTest64(false).ToString());
            Console.WriteLine("SEED 64 TEST = " + RunSeedTest64(false).ToString());
            Console.WriteLine("\r\nAll operations complete, press any key to exit...");
            Console.ReadKey(true);
        }

        /*static private UInt32 swap_uint32(UInt32 val) // endienness correction
        {
            val = ((val << 8) & 0xFF00FF00) | ((val >> 8) & 0x00FF00FF);
            return (val << 16) | (val >> 16);
        }

        static private UInt64 swap_uint64(UInt64 val)
        {
            val = (val & 0x00000000FFFFFFFF) << 32 | (val & 0xFFFFFFFF00000000) >> 32;
            val = (val & 0x0000FFFF0000FFFF) << 16 | (val & 0xFFFF0000FFFF0000) >> 16;
            val = (val & 0x00FF00FF00FF00FF) << 8 | (val & 0xFF00FF00FF00FF00) >> 8;
            return val;
        }*/

        static bool RunVectorTest32(bool displayInConsole = false)
        {
            UInt32[] seedArray = new UInt32[1];
            Isaac32 i32 = new Isaac32(new UInt32[1]); // non-seed-test
            string resultString = "";
            for (int i = 0; i < 2; i++)
            {
                i32.Shuffle(); // non-seed-test
                for (int j = 0; j < 256; j++)
                {
                    UInt32 r = i32.result[j];

                    resultString += r.ToString("x8");
                    if (displayInConsole)
                    {
                        Console.Write(r.ToString("x8") + " "); // non-seed-test
                        if (j % 8 == 7 && j > 0) Console.WriteLine("");
                    }
                }
            }
            if (displayInConsole)
            {
                Console.WriteLine("\r\nTest complete, press any key to continue...\r\n");
                Console.ReadKey(true);
            }

            if (resultString != VerifyMeVectors)
                return false;
            else
                return true;
        }

        static bool RunSeedTest32(bool displayInConsole = false)
        {
            const string seedTest = "This is <i>not</i> the right mytext.";
            byte[] seed = new ASCIIEncoding().GetBytes(seedTest);
            UInt32[] seedArray = new UInt32[30];
            string resultString = "";
            Buffer.BlockCopy(seed, 0, seedArray, 0, seed.Length * sizeof(byte)); // seed-test
            //for (int i = 0; i < seedArray.Length; i++)  //endienness correction
            //{
                //seedArray[i] = swap_uint32(seedArray[i]);
            //}
            Isaac32 i32 = new Isaac32(seedArray); // seed-test
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    UInt32 r = i32.Next();
                    resultString += r.ToString("x8");
                    if (displayInConsole)
                    {
                        Console.Write(r.ToString("x8") + " "); // seed-test
                        if (j % 8 == 7 && j > 0) Console.WriteLine("");
                    }
                }
            }
            if (displayInConsole)
            {
                Console.WriteLine("\r\nTest complete, press any key to continue...\r\n");
                Console.ReadKey(true);
            }
            if (resultString != VerifyMeSeed)
                return false;
            else
                return true;
        }

        static bool RunVectorTest64(bool displayInConsole = false)
        {
            UInt64[] seedArray = new UInt64[1];
            Isaac64 i64 = new Isaac64(new UInt64[1]); // non-seed-test
            string resultString = "";
            for (int i = 0; i < 2; i++)
            {
                i64.Shuffle(); // non-seed-test
                for (int j = 0; j < 256; j++)
                {
                    UInt64 r = i64.result[j];

                    resultString += r.ToString("x16");
                    if (displayInConsole)
                    {
                        Console.Write(r.ToString("x16") + " "); // non-seed-test
                        if (j % 8 == 7 && j > 0) Console.WriteLine("");
                    }
                }
            }
            if (displayInConsole)
            {
                Console.WriteLine("\r\nTest complete, press any key to continue...\r\n");
                Console.ReadKey(true);
            }

            if (resultString != VerifyMeVectors64)
                return false;
            else
                return true;
        }

        static bool RunSeedTest64(bool displayInConsole = false)
        {
            const string seedTest = "This is <i>not</i> the right mytext.";
            byte[] seed = new ASCIIEncoding().GetBytes(seedTest);
            UInt64[] seedArray = new UInt64[30];
            string resultString = "";
            Buffer.BlockCopy(seed, 0, seedArray, 0, seed.Length * sizeof(byte)); // seed-test
            //for (int i = 0; i < seedArray.Length; i++) // endienness correction
            //{
                //seedArray[i] = swap_uint64(seedArray[i]);
            //}
            Isaac64 i64 = new Isaac64(seedArray); // seed-test
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    UInt64 r = i64.Next();
                    resultString += r.ToString("x16");
                    if (displayInConsole)
                    {
                        Console.Write(r.ToString("x16") + " "); // seed-test
                        if (j % 8 == 7 && j > 0) Console.WriteLine("");
                    }
                }
            }
            if (displayInConsole)
            {
                Console.WriteLine("\r\nTest complete, press any key to continue...\r\n");
                Console.ReadKey(true);
            }
            if (resultString != VerifyMeSeed64)
                return false;
            else
                return true;
        }
    }
}
