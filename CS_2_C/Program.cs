using LibCS2C.Compilation;
using System;
using System.IO;
using System.Text;

namespace CS_2_C
{
    class Program
    {
        /// <summary>
        /// Entrypoint of the quick testing program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Compiler compiler = new Compiler();
            // string output = compiler.CompileProject(@"C:\Users\jeffr\Documents\visual studio 2015\Projects\Sharpen\kernel\Sharpen\Sharpen.csproj");
            StringBuilder output = compiler.CompileProject(@"C:\Users\Niels\Documents\Sharpen\kernel\Sharpen\Sharpen.csproj");
            //string output = compiler.CompileProject(@"C:\Users\Niels\Source\Repos\Sharpen\kernel\Sharpen\Sharpen.csproj");

            File.WriteAllText("output.c", output.ToString());
            Console.ReadLine();
        }
    }
}
