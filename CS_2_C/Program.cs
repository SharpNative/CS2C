using LibCS2C.Compilation;
using System;

namespace CS_2_C
{
    class Program
    {
        /// <summary>
        /// Entrypoint of the quick testing the compiler
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Compiler compiler = new Compiler();
            // compiler.CompileProject(@"C:\Users\jeffr\Documents\visual studio 2015\Projects\Sharpen\kernel\Sharpen\Sharpen.csproj", "");
            compiler.CompileProject(@"C:\Users\Niels\Documents\Sharpen\kernel\Sharpen\Sharpen.csproj", "");
            compiler.CreateHeaderFile("output.h", new string[0]);
            compiler.CreateSourceFile("output.c", new string[] { "output.h" });
            Console.ReadLine();
        }
    }
}
