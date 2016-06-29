using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TestProject.OtherNameSpace
{
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
    public struct OutsideClassStruct
    {
        int abc = 44;
    }

    enum drollen
    {
        test = 0,
        testtt = 1
    }

    class TestClass
    {
        public int testing { get; private set; } = 10;

        static int staticTest = 4;
        private int nonStaticTest = 2;
        private int test;

        public static extern void testing123();
        
        public TestClass(int test)
        {
            Console.WriteLine("we are in the constructor of shit");
            int aaa = testing;
            nonStaticTest = 56 + test;
            staticTest = test;
            this.test = test;
            testing = staticTest;

            int aze = drollen.test;

            OutsideClassStruct abc = new OutsideClassStruct();

            for (int i = 0; i < 10 && i < 15; i++)
            {
                Console.WriteLine("jeej");
            }

            for (;;)
            {
                Console.WriteLine("test");
            }
            testing123();
        }

        public unsafe void TestPointer()
        {
            int x = 42 ;
            int* ptr = &x;

            int tests = 10;

            int* address = (int*)0xB8000;
            address[0] = 3;

            int t = 0;
            int test = (int)(((t << 3) | (t >> 3)) & 22);
            test <<= 3;

            test >>= 5;
            test |= 1;
            test >>= 2;
            test = ~1337;

            if(!(test > 3))
            {
                Console.WriteLine("hisdfqksdlfj");
            }
            else if(test == 8)
            {
                t = 4;
            }
            else
            {
                Console.WriteLine("else");
            }
            
            if((address[1] == 3 && x == 88888) && x + 1 < 8 && x != 222)
            {
                Console.WriteLine("hooray");
            }

            --t;
            t *= 4;
            ++t;
            t--;
            t /= 2;
            t++;

            t -= 55;
            t += 44;

            t *= testing++;
        }

        public int getNonStaticTest()
        {
            return nonStaticTest + 5 - testing;
        }
    }
}