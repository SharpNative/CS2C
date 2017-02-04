using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.IO;
using System.Text;

namespace LibCS2C.Compilation
{
    public class Compiler
    {
        public string CurrentProjectName { get; private set; }
        public string CurrentDocumentName { get; private set; }

        private StringBuilder m_headerCode;
        private StringBuilder m_sourceCode;

        /// <summary>
        /// Compiles a project
        /// </summary>
        /// <param name="projectPath">The project path</param>
        /// <param name="initSuffix">The suffix to the init method</param>
        public void CompileProject(string projectPath, string initSuffix)
        {
            // Workspace to open project
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();
            Project project = workspace.OpenProjectAsync(projectPath).Result;
            CurrentProjectName = project.Name;

            Console.WriteLine("Project name: " + CurrentProjectName);
            Console.WriteLine("-------------");

            // Buffer that holds all the output code
            SyntaxWalker walker = new SyntaxWalker(initSuffix);

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

            m_headerCode = walker.GetHeaderCode();
            m_sourceCode = walker.GetSourceCode();
        }

        /// <summary>
        /// Ensures that a directory exists
        /// </summary>
        /// <param name="path">The path</param>
        private void ensureDirectoryExists(string path)
        {
            string outDir = Path.GetDirectoryName(path);
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
        }

        /// <summary>
        /// Writes a code file
        /// </summary>
        /// <param name="sb">The StringBuilder containing the code</param>
        /// <param name="filename">The filename</param>
        /// <param name="includeHeaders">the headers to include</param>
        private void writeCodeFile(StringBuilder sb, string filename, string[] includeHeaders)
        {
            if (sb == null)
                throw new Exception("Code is not compiled yet!");

            ensureDirectoryExists(filename);

            // Write contents
            StreamWriter stream = new StreamWriter(filename, false);

            // Additional header files to include
            foreach (string file in includeHeaders)
            {
                stream.WriteLine(string.Format("#include \"{0}\"", file));
            }

            // The output code itself
            stream.Write(sb.ToString());

            stream.Close();
        }

        /// <summary>
        /// Creates the header file
        /// </summary>
        /// <param name="filename">The header filename</param>
        /// <param name="includeHeaders">What headers to include</param>
        public void CreateHeaderFile(string filename, string[] includeHeaders)
        {
            writeCodeFile(m_headerCode, filename, includeHeaders);
        }

        /// <summary>
        /// Creates the source file
        /// </summary>
        /// <param name="filename">The source filename</param>
        /// <param name="includeHeaders">What headers to include</param>
        public void CreateSourceFile(string filename, string[] includeHeaders)
        {
            writeCodeFile(m_sourceCode, filename, includeHeaders);
        }
    }
}
