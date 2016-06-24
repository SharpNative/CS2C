using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 5;
            int b = 3;
            int c = a + b;
            b = 2;
            Method2(33, 44, 88);
            TestProject.OtherNameSpace.shit.hi();
        }

        static void Method2(int a, int b, int c)
        {
            string teststring = "lo";
        }
    }
}