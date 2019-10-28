using System;
using System.Reflection;

namespace Utilities.Attributes.SQL
{
    /// <summary>
    /// Attribute for specific how reflection should perform on usage class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BindingFlagsAttribute : Attribute
    {
        public BindingFlags BindingFlags;

        public BindingFlagsAttribute(BindingFlags bindingFlags)
        {
            this.BindingFlags = bindingFlags;
        }
    }
}