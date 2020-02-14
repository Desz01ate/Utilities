using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Classes;

namespace Utilities.SQL.Events
{
    /// <summary>
    /// Contains event for command execution interceptor.
    /// </summary>
    public static class ExecutionInterceptor
    {
        /// <summary>
        /// The event that will trigger when query is ready to execute.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public delegate void OnQueryExecutingEventHandler(string sql, IEnumerable<DatabaseParameter> parameters);
        /// <summary>
        /// The event that will trigger when query is executed.
        /// </summary>
        /// <param name="affectedRows"></param>
        public delegate void OnQueryExecutedEventHandler(int affectedRows);
    }
}
