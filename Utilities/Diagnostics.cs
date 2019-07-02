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
        public static (long before, long after, long amt) MemeryEstimation(Action action)
        {
            long beforeExecution = GC.GetTotalMemory(false) / 1024;
            action();
            long afterExecution = GC.GetTotalMemory(false) / 1024;
            return (beforeExecution, afterExecution, afterExecution - beforeExecution);
        }
        /// <summary>
        /// Execute given function and return total memory usage during an execution
        /// </summary>
        /// <param name="action">Any given function to execute</param>
        /// <returns>Total memory usage</returns>
        public static async Task<(long before, long after, long amt)> MemeryEstimationAsync(Func<Task> action)
        {
            long beforeExecution = GC.GetTotalMemory(false) / 1024;
            await action();
            long afterExecution = GC.GetTotalMemory(false) / 1024;
            return (beforeExecution, afterExecution, afterExecution - beforeExecution);
        }
        /// <summary>
        /// Execute given function and return an execution time in milliseconds
        /// </summary>
        /// <param name="action">Any given function to execute</param>
        /// <returns>An execution time in milliseconds</returns>
        public static async Task<long> RuntimeEstimationAsync(Func<Task> action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
