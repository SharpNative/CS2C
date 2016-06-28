using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.OtherNameSpace;

namespace TestProject
{
    class Program
    {
        struct TestStruct
        {
            int lol;
        }

        static void Main(string[] args)
        {
            int a = 5;
            int b = 3;
            int c = a + b;
            b = 2;
            Method2(33, 44, 88);

            TestClass nein = new TestClass(44);
        }

        static void Method2(int a, int b, int c)
        {
            Console.WriteLine("Method2!");
            string teststring = "lo";
        }
    }
}