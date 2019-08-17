using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Asp.Core.Enumerables
{
    public enum RepositoryType
    {
        /// <summary>
        /// Specific that engine will generate service with singleton pattern
        /// </summary>
        Singleton,
        /// <summary>
        /// (Recommend) Specific that engine will generate service with public ctor that allow unit of work to be dependency injection
        /// </summary>
        NonSingleton
    }
}
