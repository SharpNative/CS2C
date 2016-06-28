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

        public static string CompileProject(string path)
        {

            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project project = workspace.OpenProjectAsync("../../../TestProject/TestProject.csproj").Result;

            Console.WriteLine("Project name: " + project.Name);
            Console.WriteLine("-------------");

            FormattedStringBuilder sb = new FormattedStringBuilder();

            foreach (Document document in project.Documents)
            {
                Console.WriteLine("File: " + document.Name);
                SyntaxWalker walker = new SyntaxWalker(sb, document.GetSemanticModelAsync().Result);
                SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
                SyntaxNode node = tree.GetRoot();
                walker.Visit(node);
                Console.WriteLine("End of file: " + document.Name);
                Console.WriteLine("------------");
            }

            return sb.ToString();
        }
    }
}
