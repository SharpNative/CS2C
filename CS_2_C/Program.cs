using System;
using System.IO;

using LibCS2C;

namespace CS_2_C
{
    class Program
    {
        /// <summary>
        /// Entrypoint
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string output = Compiler.CompileProject("../../../TestProject/TestProject.csproj");
            
            File.WriteAllText("output.c", output);
            Console.ReadLine();
        }
    }
}
