using System;

namespace LibCS2C.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class Extern : Attribute
    {
        public string ExternalName { get; private set; }

        /// <summary>
        /// Initializes a new external method reference
        /// </summary>
        /// <param name="overrideName">The name of the external method reference</param>
        public Extern(string externalName)
        {
            ExternalName = externalName;
        }
    }
}
