using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C
{
    public class Compiler
    {
        /// <summary>
        /// Compiles a project
        /// </summary>
        /// <param name="path">The project path</param>
        /// <returns></returns>
        public static string CompileProject(string path)
        {
            // Workspace to open project
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project project = workspace.OpenProjectAsync(path).Result;

            Console.WriteLine("Project name: " + project.Name);
            Console.WriteLine("-------------");

            // Buffer that holds all the output code
            SyntaxWalker walker = new SyntaxWalker();

            // Loop through each file of the project
            foreach (Document document in project.Documents)
            {
                Console.WriteLine("File: " + document.Name);

                walker.SetDocument(document);

                // Go through the syntax tree and convert the code
                SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
                SyntaxNode node = tree.GetRoot();
                walker.Visit(node);

                Console.WriteLine("End of file: " + document.Name);
                Console.WriteLine("------------");
            }
            
            return walker.ToString();
        }
    }
}
