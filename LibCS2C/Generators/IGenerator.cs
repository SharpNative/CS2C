using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Generators
{
    interface IGenerator<T> where T: class
    {
        void Generate(T node);
    }
}
