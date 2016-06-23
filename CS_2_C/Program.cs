using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace CS_2_C
{
    class Program
    {
        static void Main(string[] args)
        {
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project project = workspace.OpenProjectAsync("../../../OS/OS.csproj").Result;

            Console.WriteLine("Project name: " + project.Name);

            FormattedStringBuilder sb = new FormattedStringBuilder();
            SyntaxWalker walker = new SyntaxWalker(sb);
            foreach(Document document in project.Documents)
            {
                Console.WriteLine("->" + document.Name);
                SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
                SyntaxNode node = tree.GetRoot();
                walker.Visit(node);
            }

            Console.Write(sb.ToString());
            Console.ReadLine();
        }
    }
}
