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
        static void Main(string[] args)
        {
            ITest t = new Test();
            t.abc();

            Test2 a = new Test3();
            a.def();


            goto quel;

            Test3 b = (Test3)a;
            b.def();

            broken:
            {
                int a = 3;
                int b = 4;
            }

        quel:
            break;
        }
    }
}