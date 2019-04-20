using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
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
