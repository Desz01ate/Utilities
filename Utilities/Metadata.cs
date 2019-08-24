using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

namespace Utilities
{
    public static class Metadata
    {
        /// <summary>
        /// Library version.
        /// </summary>
        public static string Version => ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyFileVersionAttribute), false)).Version;
        /// <summary>
        /// Build target framework version.
        /// </summary>
        public static string TargetFramework => (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false).First() as TargetFrameworkAttribute)?.FrameworkName;
    }
}
