using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    /// <summary>
    /// Simple diagnostic methods
    /// </summary>
    public static class Diagnostics
    {
        /// <summary>
        /// Execute given function and return an execution time in milliseconds
        /// </summary>
        /// <param name="action">Any given function to execute</param>
        /// <returns>An execution time in milliseconds</returns>
        public static long RuntimeEstimation(Action action)
        {
            if (action == null) return 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        /// <summary>
        /// Execute given function and return total memory usage during an execution
        /// </summary>
        /// <param name="action">Any given function to execute</param>
        /// <returns>Total memory usage</returns>
        public static (long Before, long After, long Amount) MemeryEstimation(Action action)
        {
            if (action == null) return (0, 0, 0);
            long beforeExecution = GC.GetTotalMemory(false) / 1024;
            action();
            long afterExecution = GC.GetTotalMemory(false) / 1024;
            return (beforeExecution, afterExecution, beforeExecution - afterExecution);
        }
        /// <summary>
        /// Execute given function and return total memory usage during an execution
        /// </summary>
        /// <param name="action">Any given function to execute</param>
        /// <returns>Total memory usage</returns>
        public static async Task<(long Before, long After, long Amount)> MemeryEstimationAsync(Func<Task> action)
        {
            if (action == null) return (0, 0, 0);
            long beforeExecution = GC.GetTotalMemory(false) / 1024;
            await action().ConfigureAwait(false);
            long afterExecution = GC.GetTotalMemory(false) / 1024;
            return (beforeExecution, afterExecution, beforeExecution - afterExecution);

        }
        /// <summary>
        /// Execute given function and return an execution time in milliseconds
        /// </summary>
        /// <param name="action">Any given function to execute</param>
        /// <returns>An execution time in milliseconds</returns>
        public static async Task<long> RuntimeEstimationAsync(Func<Task> action)
        {
            if (action == null) return 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await action().ConfigureAwait(false);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
