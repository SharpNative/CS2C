using LibCS2C.Context;

namespace LibCS2C.Generators
{
    public abstract class GeneratorBase<T> : IGenerator<T> where T: class
    {
        protected WalkerContext m_context;

        public abstract void Generate(T node);
    }
}
