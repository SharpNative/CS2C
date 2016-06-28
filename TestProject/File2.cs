using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.OtherNameSpace
{
    class TestClass
    {
        public int testing { get; private set; } = 10;

        static int staticTest = 4;
        private int nonStaticTest = 2;
        private int test;
        
        public TestClass(int test)
        {
            Console.WriteLine("we are in the constructor of shit");
            int aaa = testing;
            nonStaticTest = 56 + test;
            staticTest = test;
            this.test = test;
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
        }

        public int getNonStaticTest()
        {
            return nonStaticTest;
        }
    }
}