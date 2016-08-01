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

            int[][] arrayOfArray = new int[5][];
            for(int i = 0; i < 5; i++)
            {
                arrayOfArray[i] = new int[10];
            }
        }
    }
}