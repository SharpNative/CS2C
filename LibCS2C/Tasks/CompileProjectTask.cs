using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibCS2C.Tasks
{
    public class CompileProjectTask : Task
    {
        private string m_projectpath;
        private string m_outputpath;

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


        public override bool Execute()
        {
            // Log a high-importance comment
            Log.LogMessage(MessageImportance.High,
                "CS2C Compiler: starting compiling \"" + Path + "\".");

            try
            {
                var output = Compiler.CompileProject(Path);

                string outDir = System.IO.Path.GetDirectoryName(Outpath);

                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);

                File.WriteAllText(m_outputpath, output);

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
