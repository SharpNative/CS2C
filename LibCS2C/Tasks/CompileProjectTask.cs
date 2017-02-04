using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;
using LibCS2C.Compilation;
using System.IO;

namespace LibCS2C.Tasks
{
    public class CompileProjectTask : Task
    {
        [Required]
        public string Path { get; private set; }

        [Required]
        public string SourceOutPath { get; private set; }

        [Required]
        public string HeaderOutPath { get; private set; }

        public string AfterBuildCommand { get; private set; }
        public string AfterBuildWorkingDir { get; private set; }
        public string HeaderFiles { get; private set; }
        public string InitSuffix { get; private set; }

        /// <summary>
        /// Executes the compiler
        /// </summary>
        /// <returns>If the compilation was successful</returns>
        public override bool Execute()
        {
            // Log a high-importance comment
            Log.LogMessage(MessageImportance.High, "CS2C Compiler: Compiling project \"" + Path + "\".");

            // Header files
            string[] headers = new string[0];
            if (HeaderFiles != null)
                headers = HeaderFiles.Split(' ');

            Compiler compiler = new Compiler();
            try
            {
                compiler.CompileProject(Path, (InitSuffix == null) ? "" : InitSuffix);
                compiler.CreateHeaderFile(HeaderOutPath, headers);
                compiler.CreateSourceFile(SourceOutPath, new string[] { Directory.GetCurrentDirectory() + "/" + HeaderOutPath });

                // Command after building is done
                if (AfterBuildCommand != null)
                {
                    Log.LogMessage(MessageImportance.High, "Running command after build: " + AfterBuildCommand);

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
