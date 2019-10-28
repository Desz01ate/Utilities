using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Utilities.SQL.Extension
{
    /// <summary>
    /// Contains extension method for sql parameter.
    /// </summary>
    public static class SqlParameterExtension
    {
        /// <summary>
        /// Convert sql parameters to Dapper-supported parameters;
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DynamicParameters ToDapperParameters<TParameter>(this IEnumerable<TParameter> parameters)
            where TParameter : DbParameter, new()
        {
            if (parameters == null)
            {
                return null;
            }
            var dapperParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dapperParameters.Add(parameter.ParameterName, parameter.Value == DBNull.Value ? null : parameter.Value);
            }
            return dapperParameters;
        }

        /// <summary>
        /// Convert sql parameter to Dapper-supported parameter;
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DynamicParameters ToDapperParameters<TParameter>(this TParameter parameter)
            where TParameter : DbParameter, new()
        {
            if (parameter == null)
            {
                return null;
            }
            var dapperParameters = new DynamicParameters();
            dapperParameters.Add(parameter.ParameterName, parameter.Value == DBNull.Value ? null : parameter.Value);
            return dapperParameters;
        }
    }
}