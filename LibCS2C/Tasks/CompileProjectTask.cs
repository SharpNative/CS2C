using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using LibCS2C.Compilation;

namespace LibCS2C.Tasks
{
    public class CompileProjectTask : Task
    {
        private string m_projectpath;
        private string m_outputpath;
        private string m_afterbuildcommand;
        private string m_afterworkingdir;

        [Required]
        public string Path
        {
            get
            {
                return m_projectpath;
            }
            set
            {
                m_projectpath = value;
            }
        }

        [Required]
        public string Outpath
        {
            get
            {
                return m_outputpath;
            }
            set
            {
                m_outputpath = value;
            }
        }

        public string AfterBuildCommand
        {
            get
            {
                return m_afterbuildcommand;
            }

            set
            {
                m_afterbuildcommand = value;
            }
        }

        public string AfterBuildWorkingDir
        {
            get
            {
                return m_afterworkingdir;
            }

            set
            {
                m_afterworkingdir = value;
            }
        }

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
                string output = compiler.CompileProject(Path);

                // Create output directory if it doesn't exist
                string outDir = System.IO.Path.GetDirectoryName(Outpath);
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);

                // Contents
                File.WriteAllText(m_outputpath, output);

                // Command after building is done
                if (m_afterbuildcommand != null)
                {
                    Log.LogMessage(MessageImportance.High, "Running after build command " + m_afterbuildcommand);

                    ProcessStartInfo info = new ProcessStartInfo(m_afterbuildcommand);
                    if (m_afterworkingdir != null)
                        info.WorkingDirectory = m_afterworkingdir;

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
