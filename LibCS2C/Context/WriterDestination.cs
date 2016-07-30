using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCS2C.Context
{
    public enum WriterDestination
    {
        Enums,
        StructPrototypes,
        Structs,
        ClassStructs,
        Delegates,
        MethodPrototypes,
        MethodDeclarations,
        TempBuffer,
        PostBuffer
    }
}
