using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TestProject.OtherNameSpace
{
    public interface ITest
    {
        void abc();
    }

    public class Test : ITest
    {
        public void abc()
        {
            int a = 3;

        }
    }

    public class Test2
    {
        public virtual void def()
        {

        }
    }

    public class Test3:Test2
    {
        public override void def()
        {

        }
    }
}