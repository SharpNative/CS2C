using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using LibCS2C.Compilation;
using System.Text;

namespace LibCS2C.Tasks
{
    public class CompileProjectTask : Task
    {
        [Required]
        public string Path { get; private set; }

        [Required]
        public string Outpath { get; private set; }

        public string AfterBuildCommand { get; private set; }
        public string AfterBuildWorkingDir { get; private set; }
        public string HeaderFiles { get; private set; }

        /// <summary>
        /// Executes the compiler
        /// </summary>
        /// <returns>If the compilation was successful</returns>
        public override bool Execute()
        {
            // Log a high-importance comment
            Log.LogMessage(MessageImportance.High, "CS2C Compiler: Compiling project \"" + Path + "\".");

            Compiler compiler = new Compiler();
            try
            {
                StringBuilder output = compiler.CompileProject(Path);

                // Create output directory if it doesn't exist
                string outDir = System.IO.Path.GetDirectoryName(Outpath);
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);

                // Write contents
                StreamWriter stream = new StreamWriter(Outpath, false);

                // Additional header files to include
                if (HeaderFiles != null)
                {
                    string[] files = HeaderFiles.Split(' ');
                    foreach (string file in files)
                    {
                        stream.WriteLine(string.Format("#include \"{0}\"", file));
                    }
                }

                // The output code itself
                stream.Write(output.ToString());

                stream.Close();

                // Command after building is done
                if (AfterBuildCommand != null)
                {
                    Log.LogMessage(MessageImportance.High, "Running after build command " + AfterBuildCommand);

                    ProcessStartInfo info = new ProcessStartInfo(AfterBuildCommand);
                    if (AfterBuildWorkingDir != null)
                        info.WorkingDirectory = AfterBuildWorkingDir;

                    Process.Start(info);
                }

                Log.LogMessage(MessageImportance.High, "Finished compiling");
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                Log.LogError("StackTrace: " + e.StackTrace);
                Log.LogError("Error occurred in file: " + compiler.CurrentDocumentName);
                return false;
            }

            return true;
        }
    }
}
