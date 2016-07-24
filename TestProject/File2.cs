using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TestProject.OtherNameSpace
{
    public class ParentClass
    {
        public ParentClass()
        {

        }

        public virtual void abc()
        {
            int abc = 123 & 5;
            if (abc < 456)
                abc--;
            else if (abc == 1)
                abc++;
            else if(abc >= 8 && abc <= 10)
            {
                abc *= 64;
            }
            else
            {
                abc = 0;
            }
        }
    }

    public class TestClass : ParentClass
    {
        public int TestProperty { get; set; } = 5;

        public TestClass(int test)
        {
        }

        public void Test()
        {
            TestProperty = 22;
        }

        public void Test(int a)
        {
            TestProperty = a * 5 - TestProperty;
        }

        public override void abc()
        {
            base.abc();
        }
    }
}