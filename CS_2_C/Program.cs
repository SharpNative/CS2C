using System;
using System.IO;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

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
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project project = workspace.OpenProjectAsync("../../../TestProject/TestProject.csproj").Result;
            
            Console.WriteLine("Project name: " + project.Name);
            Console.WriteLine("-------------");

            FormattedStringBuilder sb = new FormattedStringBuilder();
            SyntaxWalker walker = new SyntaxWalker(sb);
            foreach(Document document in project.Documents)
            {
                Console.WriteLine("File: " + document.Name);
                SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
                SyntaxNode node = tree.GetRoot();
                walker.Visit(node);
                Console.WriteLine("End of file: " + document.Name);
                Console.WriteLine("------------");
            }

            File.WriteAllText("output.c", sb.ToString());
        }
    }
}
