using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;
using System.IO;

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

        public override bool Execute()
        {
            // Log a high-importance comment
            Log.LogMessage(MessageImportance.High,
                "CS2C Compiler: starting compiling \"" + Path + "\".");

            try
            {
                var output = Compiler.Compiler.CompileProject(Path);

                string outDir = System.IO.Path.GetDirectoryName(Outpath);

                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);

                File.WriteAllText(m_outputpath, output);

                if(m_afterbuildcommand != null)
                {
                    Log.LogMessage(MessageImportance.High, "Running after build command " + m_afterbuildcommand);

                    ProcessStartInfo info = new ProcessStartInfo(m_afterbuildcommand);
                    if (m_afterworkingdir != null)
                        info.WorkingDirectory = m_afterworkingdir;

                    Process.Start(info);
                }

                Log.LogMessage(MessageImportance.High, "Finished compiling");
            }
            catch(Exception e)
            {
                Log.LogErrorFromException(e);
            }

            return true;
        }
    }
}
