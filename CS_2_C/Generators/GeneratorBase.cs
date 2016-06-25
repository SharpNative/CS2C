using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_2_C.Generators
{
    abstract class GeneratorBase<T> : IGenerator<T> where T: class
    {
        protected WalkerContext m_context;

        public abstract void Generate(T node);
    }
}
