using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;

namespace LibCS2C.Compilation
{
    public class Compiler
    {
        public string CurrentProjectName { get; private set; }
        public string CurrentDocumentName { get; private set; }

        /// <summary>
        /// Compiles a project
        /// </summary>
        /// <param name="path">The project path</param>
        /// <returns></returns>
        public string CompileProject(string path)
        {
            // Workspace to open project
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project project = workspace.OpenProjectAsync(path).Result;
            CurrentProjectName = project.Name;

            Console.WriteLine("Project name: " + CurrentProjectName);
            Console.WriteLine("-------------");

            // Buffer that holds all the output code
            SyntaxWalker walker = new SyntaxWalker();

            // Loop through each file of the project
            foreach (Document document in project.Documents)
            {
                CurrentDocumentName = document.Name;
                Console.WriteLine("Process file: " + CurrentDocumentName);

                walker.SetDocument(document);

                // Go through the syntax tree and convert the code
                SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
                SyntaxNode node = tree.GetRoot();
                walker.Visit(node);
            }
            
            return walker.ToString();
        }
    }
}
