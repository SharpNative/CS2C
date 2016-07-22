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
            TestClass nein = new TestClass(44);
            nein.Test();

            nein.TestProperty = 42;
            short val = 42;
            nein.Test(val);

            int a = 55;
            switch(a)
            {
                case 3:
                {
                    break;
                }

                case 6:
                    a += 5;
                    if(a > 2)
                    {
                        a -= 2;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}