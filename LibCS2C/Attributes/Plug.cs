using System;

namespace LibCS2C.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class Plug : Attribute
    {
        public string OverrideName { get; private set; }

        /// <summary>
        /// Initializes a new plug
        /// </summary>
        /// <param name="overrideName">The name of the method to override</param>
        public Plug(string overrideName)
        {
            OverrideName = overrideName;
        }
    }
}
