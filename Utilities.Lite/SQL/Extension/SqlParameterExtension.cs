#if !NET45
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Utilities.SQL.Extension
{
    public static class SqlParameterExtension
    {
        /// <summary>
        /// Convert vanilla sql parameters to dapper-supported parameters;
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DynamicParameters ToDapperParameters<TParameter>(this IEnumerable<TParameter> parameters)
            where TParameter : DbParameter, new()
        {
            var dapperParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dapperParameters.Add(parameter.ParameterName, parameter.Value);
            }
            return dapperParameters;
        }
    }
}
#endif