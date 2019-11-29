using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Enum
{
    /// <summary>
    /// Optimizer level.
    /// </summary>
    public enum SqlMapperOptimize
    {
        /// <summary>
        /// Do not optimize the mapping process.
        /// </summary>
        None,
        /// <summary>
        /// Auto detect how the mapper should perform between the aggressive and passive.
        /// </summary>
        Auto,
        /// <summary>
        /// Passive optimize for well-balanced between the small and large dataset.
        /// </summary>
        Passive,
        /// <summary>
        /// Aggressive optimize to the highest level, may perform worst when dataset is very small.
        /// </summary>
        Aggressive
    }
}
