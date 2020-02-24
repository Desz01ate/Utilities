using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Utilities.Classes;
using Utilities.Enum;
using Utilities.Interfaces;

namespace Utilities.SQL.Abstracts
{
    /// <summary>
    /// Abstract class that is contains the higher-abstract level of IDatabaseConnector.
    /// </summary>
    public abstract class DatabaseConnectorBase : IDatabaseConnector
    {
        public abstract string ConnectionString { get; }
        public virtual bool IsOpen => Connection?.State == ConnectionState.Open;
        public abstract DbConnection Connection { get; }
        internal protected abstract string CompatibleFunctionName(SqlFunction function);
        internal protected abstract string CompatibleSQLType(Type type);
        private bool Disposed { get; set; }
        void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
            {
                Connection.Dispose();
            }
            Disposed = true;
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public abstract int ExecuteNonQuery(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        public abstract Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        public abstract IEnumerable<T> ExecuteReader<T>(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false) where T : class, new();
        public abstract IEnumerable<dynamic> ExecuteReader(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false);
        public abstract DataTable ExecuteReaderAsDataTable(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        public abstract Task<DataTable> ExecuteReaderAsDataTableAsync(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        public abstract Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false) where T : class, new();
        public abstract Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text, bool buffered = false);
        public abstract object ExecuteScalar(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        public abstract T ExecuteScalar<T>(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        public abstract Task<object> ExecuteScalarAsync(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text);
        public abstract Task<T> ExecuteScalarAsync<T>(string sql, IEnumerable<DatabaseParameter>? parameters = null, DbTransaction? transaction = null, CommandType commandType = CommandType.Text);
    }
}
