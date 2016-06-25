using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.OtherNameSpace
{
    class TestClass
    {
        static int staticTest = 4;
        private int nonStaticTest = 2;

        

        public TestClass(int test)
        {
            Console.WriteLine("we are in the constructor of shit");
            nonStaticTest = 56;
            staticTest = test;
        }

        public int getNonStaticTest()
        {
            return nonStaticTest;
        }
    }
}