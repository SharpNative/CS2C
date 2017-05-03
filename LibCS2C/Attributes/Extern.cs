using System;

namespace LibCS2C.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class Extern : Attribute
    {
        public string ExternalName { get; private set; }
        public bool CreatePrototype { get; private set; }

        /// <summary>
        /// Initializes a new external method reference
        /// </summary>
        /// <param name="externalName">The name of the external method reference</param>
        /// <param name="CreatePrototype">If we need to create a prototype</param>
        public Extern(string externalName, bool createPrototype = false)
        {
            ExternalName = externalName;
            CreatePrototype = createPrototype;
        }
    }
}
