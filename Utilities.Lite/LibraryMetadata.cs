using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace Utilities
{
    /// <summary>
    /// Contains information of Utilities library version.
    /// </summary>
    public static class LibraryMetadata
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