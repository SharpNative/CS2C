﻿using System;
using System.IO;
using LibCS2C.Compiler;

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
            // string output = Compiler.CompileProject("../../../TestProject/TestProject.csproj");
            // string output = Compiler.CompileProject(@"C:\Users\jeffr\Documents\visual studio 2015\Projects\Sharpen\kernel\Sharpen\Sharpen.csproj");
            // string output = Compiler.CompileProject(@"C:\Users\Niels\Documents\Sharpen\kernel\Sharpen\Sharpen.csproj");
            string output = Compiler.CompileProject(@"C:\Users\jeffr\Source\Repos\Sharpen\user\shell\shell\shell.csproj");

            File.WriteAllText("output.c", output);
            Console.ReadLine();
        }
    }
}
